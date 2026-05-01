using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionBillingSystem.Domain.Common;
using SubscriptionBillingSystem.Infrastructure.Persistence;
using SubscriptionBillingSystem.Infrastructure.Persistence.Outbox;
using System.Text.Json;

namespace SubscriptionBillingSystem.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Outbox Processor
    /// -----------------
    /// Responsible for reliably processing domain events stored in the Outbox table.
    /// 
    /// This ensures:
    /// - No event loss (durable persistence before processing)
    /// - Retry support for transient failures
    /// - Exponential backoff for failed events
    /// - Eventual consistency between domain and external side effects
    /// </summary>
    public class OutboxProcessor
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;

        private const int BatchSize = 50;

        public OutboxProcessor(ApplicationDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        /// <summary>
        /// Main processing loop for Outbox messages.
        /// Executes in batches and applies retry logic for failed messages.
        /// </summary>
        public async Task ProcessAsync(CancellationToken cancellationToken = default)
        {
            // Fetch only messages that:
            // 1. Are not processed yet
            // 2. Are eligible for retry (NextRetryAt is null or reached)
            var messages = await _context.OutboxMessages
                .Where(x =>
                    !x.IsProcessed &&
                    (x.NextRetryAt == null || x.NextRetryAt <= DateTime.UtcNow))
                .OrderBy(x => x.OccurredOn)
                .Take(BatchSize)
                .ToListAsync(cancellationToken);

            if (!messages.Any())
                return;

            var hasChanges = false;

            foreach (var message in messages)
            {
                try
                {
                    // Attempt to process and publish the domain event
                    await ProcessMessage(message, cancellationToken);

                    // Mark message as successfully processed
                    message.IsProcessed = true;
                    message.ProcessedOn = DateTime.UtcNow;
                    message.LastError = null;

                    hasChanges = true;
                }
                catch (Exception ex)
                {
                    // Increment retry count for failed processing
                    message.RetryCount++;
                    message.LastError = ex.Message;

                    // Apply exponential backoff strategy for retry scheduling
                    message.NextRetryAt = DateTime.UtcNow
                        .AddMinutes(Math.Pow(2, message.RetryCount));

                    // Stop retrying after max attempts reached
                    if (message.RetryCount >= message.MaxRetryCount)
                    {
                        // Mark as completed to avoid infinite retry loops
                        message.IsProcessed = true;
                    }

                    hasChanges = true;
                }
            }

            // Persist all state changes:
            // - success updates
            // - retry updates
            // - failure metadata
            if (hasChanges)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Processes a single Outbox message:
        /// - Deserializes stored domain event
        /// - Publishes it using MediatR
        /// </summary>
        private async Task ProcessMessage(
            OutboxMessage message,
            CancellationToken cancellationToken)
        {
            var type = Type.GetType(message.Type);

            if (type == null)
                throw new Exception($"Unknown event type: {message.Type}");

            // Deserialize event payload into strongly-typed domain event            
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            var domainEvent = JsonSerializer.Deserialize(message.Content, type, jsonOptions);
            
            if (domainEvent == null)
                throw new Exception("Failed to deserialize event");

            // Generic validation (decoupled, domain-driven)
            if (domainEvent is IValidatableEvent validatable)
            {
                validatable.Validate();
            }
            else
            {
                throw new Exception($"Event {type.Name} does not implement validation");
            }

            // Publish event to all registered handlers
            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }
}