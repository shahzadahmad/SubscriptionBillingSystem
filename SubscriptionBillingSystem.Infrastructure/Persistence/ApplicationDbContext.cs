using Microsoft.EntityFrameworkCore;
using SubscriptionBillingSystem.Application.Common.Interfaces;
using SubscriptionBillingSystem.Domain.Aggregates.CustomerAggregate;
using SubscriptionBillingSystem.Domain.Aggregates.InvoiceAggregate;
using SubscriptionBillingSystem.Domain.Aggregates.SubscriptionAggregate;
using SubscriptionBillingSystem.Domain.Common;
using SubscriptionBillingSystem.Infrastructure.Persistence.Outbox;

namespace SubscriptionBillingSystem.Infrastructure.Persistence
{
    /// <summary>
    /// Main EF Core DbContext for the application.
    /// 
    /// Responsibilities:
    /// - Acts as Unit of Work
    /// - Manages entity persistence
    /// - Captures Domain Events and stores them in Outbox (for eventual consistency)
    /// - Applies entity configurations
    /// </summary>
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        // Optional dispatcher (commented out for now)
        // Used if you want to publish domain events immediately after DB commit
        //private readonly DomainEventDispatcher _dispatcher;

        /// <summary>
        /// Constructor for DbContext
        /// </summary>
        /// <param name="options">EF Core DbContext options</param>
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options
        //,DomainEventDispatcher dispatcher
        ) : base(options)
        {
            //_dispatcher = dispatcher;
        }

        #region DbSets (Tables)

        /// <summary>
        /// Customers table
        /// </summary>
        public DbSet<Customer> Customers => Set<Customer>();

        /// <summary>
        /// Subscriptions table
        /// </summary>
        public DbSet<Subscription> Subscriptions => Set<Subscription>();

        /// <summary>
        /// Invoices table
        /// </summary>
        public DbSet<Invoice> Invoices => Set<Invoice>();

        /// <summary>
        /// Outbox messages table (used for reliable event publishing)
        /// </summary>
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        #endregion

        /// <summary>
        /// Overrides SaveChangesAsync to:
        /// 1. Capture domain events from aggregate roots
        /// 2. Store them in Outbox table (Outbox Pattern)
        /// 3. Fix incorrect EF Core state tracking for Invoice entity
        /// 4. Persist all changes in a single transaction
        /// 5. Clear domain events after successful commit
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            #region 1. Extract Domain Events

            // Get all tracked aggregate roots that have domain events
            var domainEntities = ChangeTracker
                .Entries<AggregateRoot>()
                .Where(x => x.Entity.DomainEvents.Any())
                .ToList();

            // Flatten all domain events into a single list
            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            #endregion

            #region 2. Convert Domain Events to Outbox Messages

            // Convert each domain event into an OutboxMessage
            // This ensures reliable event delivery (eventual consistency)
            var outboxMessages = domainEvents.Select(domainEvent => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = domainEvent.GetType().FullName!, // Used for deserialization
                Content = System.Text.Json.JsonSerializer.Serialize(domainEvent), // Serialized event payload
                OccurredOn = DateTime.UtcNow,
                IsProcessed = false // Will be processed by background worker
            });

            // Add Outbox messages to DbContext
            await Set<OutboxMessage>().AddRangeAsync(outboxMessages, cancellationToken);

            #endregion

            #region 3. Fix EF Core Tracking Issues (Invoice)

            // Ensure EF Core correctly identifies INSERT vs UPDATE for Invoice
            FixInvoiceStates();

            #endregion

            #region 4. Persist Changes

            // Save all changes (Entities + OutboxMessages) in one transaction
            var result = await base.SaveChangesAsync(cancellationToken);

            #endregion

            #region 5. Dispatch Domain Events (Optional)

            // If using immediate dispatch instead of Outbox
            // await _dispatcher.DispatchAsync(domainEvents);

            #endregion

            #region 6. Clear Domain Events

            // Clear domain events after successful commit
            // Prevents duplicate processing
            foreach (var entity in domainEntities)
            {
                entity.Entity.ClearDomainEvents();
            }

            #endregion

            return result;
        }

        /// <summary>
        /// Configures entity models using Fluent API
        /// Automatically applies all configurations from the current assembly
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply all IEntityTypeConfiguration<T> from assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Fixes incorrect EF Core state tracking for Invoice entity.
        /// 
        /// Problem:
        /// EF Core may incorrectly mark a new entity as "Modified"
        /// instead of "Added", especially in disconnected scenarios (e.g., APIs).
        /// 
        /// Solution:
        /// - If entity is marked Modified but doesn't exist in DB → change to Added
        /// - If entity exists → keep as Modified
        /// - If entity is Detached → treat as Added
        /// </summary>
        private void FixInvoiceStates()
        {
            // Get all tracked Invoice entities
            var invoiceEntries = ChangeTracker
                .Entries<Invoice>();

            foreach (var entry in invoiceEntries)
            {
                // Handle Modified state
                if (entry.State == EntityState.Modified)
                {
                    // Check if entity actually exists in database
                    var existsInDb = Invoices
                        .AsNoTracking()
                        .Any(i => i.Id == entry.Entity.Id);

                    if (!existsInDb)
                    {
                        // Entity does NOT exist → should be INSERT
                        entry.State = EntityState.Added;
                    }
                    else
                    {
                        // Entity exists → valid UPDATE
                        entry.State = EntityState.Modified;
                    }
                }

                // Handle Detached state (not tracked by EF)
                if (entry.State == EntityState.Detached)
                {
                    // Treat as new entity → INSERT
                    entry.State = EntityState.Added;
                }
            }
        }
    }
}