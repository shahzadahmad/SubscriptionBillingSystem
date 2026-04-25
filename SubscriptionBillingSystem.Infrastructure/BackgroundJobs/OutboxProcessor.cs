using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionBillingSystem.Infrastructure.Persistence;
using SubscriptionBillingSystem.Infrastructure.Persistence.Outbox;

namespace SubscriptionBillingSystem.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Handles processing of Outbox messages with retry support and safe execution.
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
        /// Fetches pending Outbox messages, processes them, and applies retry logic on failure.
        /// </summary>
        public async Task ProcessAsync()
        {
            // Get pending messages that are ready to be retried
            var messages = await _context.OutboxMessages
                .Where(x =>
                    !x.IsProcessed &&
                    (x.NextRetryAt == null || x.NextRetryAt <= DateTime.UtcNow))
                .OrderBy(x => x.OccurredOn)
                .Take(BatchSize)
                .ToListAsync();

            foreach (var message in messages)
            {
                try
                {
                    // Process and publish event
                    await ProcessMessage(message);

                    // Mark as successfully processed
                    message.IsProcessed = true;
                    message.ProcessedOn = DateTime.UtcNow;
                    message.LastError = null;
                }
                catch (Exception ex)
                {
                    // Increase retry count on failure
                    message.RetryCount++;
                    message.LastError = ex.Message;

                    // Apply exponential backoff for next retry attempt
                    message.NextRetryAt = DateTime.UtcNow
                        .AddMinutes(Math.Pow(2, message.RetryCount));

                    // Stop retrying after max attempts reached
                    if (message.RetryCount >= message.MaxRetryCount)
                    {
                        message.IsProcessed = false;
                    }
                }
            }

            // Persist all updates (status, retries, timestamps)
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deserializes and publishes a single Outbox message via MediatR.
        /// </summary>
        private async Task ProcessMessage(OutboxMessage message)
        {
            // Resolve event type dynamically            
            var type = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                            .FirstOrDefault(t => t.FullName == message.Type);

            if (type == null)
                throw new Exception($"Unknown event type: {message.Type}");

            // Deserialize stored event payload
            var domainEvent = System.Text.Json.JsonSerializer.Deserialize(
                message.Content,
                type);

            if (domainEvent == null)
                throw new Exception("Failed to deserialize event");

            // Publish event to registered handlers
            await _mediator.Publish(domainEvent);
        }
    }
}