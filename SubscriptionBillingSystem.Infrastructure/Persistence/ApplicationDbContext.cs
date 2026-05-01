using Microsoft.EntityFrameworkCore;
using SubscriptionBillingSystem.Application.Common.Interfaces;
using SubscriptionBillingSystem.Domain.Aggregates.CustomerAggregate;
using SubscriptionBillingSystem.Domain.Aggregates.InvoiceAggregate;
using SubscriptionBillingSystem.Domain.Aggregates.SubscriptionAggregate;
using SubscriptionBillingSystem.Domain.Common;
using SubscriptionBillingSystem.Infrastructure.Persistence.Outbox;
using System.Text.Json;

namespace SubscriptionBillingSystem.Infrastructure.Persistence
{
    /// <summary>
    /// EF Core DbContext
    /// ------------------
    /// Responsibilities:
    /// - Acts as Unit of Work
    /// - Persists aggregates
    /// - Captures domain events
    /// - Stores events in Outbox for reliable processing
    /// </summary>
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        #region DbSets

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Subscription> Subscriptions => Set<Subscription>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        #endregion

        /// <summary>
        /// Saves changes and ensures domain events are persisted
        /// using Outbox pattern for reliability.
        /// </summary>
        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            // 1. Collect domain events from aggregates
            var domainEntities = ChangeTracker
                .Entries<AggregateRoot>()
                .Where(x => x.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            // 2. Convert domain events to Outbox messages
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            var outboxMessages = domainEvents.Select(domainEvent => new OutboxMessage
            {
                Id = Guid.NewGuid(),

                // MUST use AssemblyQualifiedName
                Type = domainEvent.GetType().AssemblyQualifiedName!,

                // Use consistent serialization options                
                Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), jsonOptions),

                OccurredOn = DateTime.UtcNow,
                IsProcessed = false
            });

            await Set<OutboxMessage>()
                .AddRangeAsync(outboxMessages, cancellationToken);

            // 3. Persist everything in a single transaction
            var result = await base.SaveChangesAsync(cancellationToken);

            // 4. Clear domain events after successful commit
            foreach (var entity in domainEntities)
            {
                entity.Entity.ClearDomainEvents();
            }

            return result;
        }

        /// <summary>
        /// Applies EF Core configurations from assembly
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}