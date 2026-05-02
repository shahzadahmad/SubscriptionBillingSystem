using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SubscriptionBillingSystem.Application.Common.Interfaces.Persistence;
using SubscriptionBillingSystem.Domain.Aggregates.SubscriptionAggregate;

namespace SubscriptionBillingSystem.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Background service that processes subscription billing cycles daily.
    /// </summary>
    public class BillingCycleJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public BillingCycleJob(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Runs continuously and triggers billing processing every 24 hours.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessBillingCycle(stoppingToken);

                // Wait before next billing cycle execution
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        /// <summary>
        /// Processes all active subscriptions and generates invoices if needed.
        /// </summary>
        private async Task ProcessBillingCycle(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var readDb = scope.ServiceProvider.GetRequiredService<IReadDbContext>();
            var aggregateContext = scope.ServiceProvider.GetRequiredService<IAggregateContext>();

            var today = DateTime.UtcNow.Date;


            // ONLY IDs from read side
            var subscriptionIds = await readDb.GetActiveSubscriptionIdsAsync(cancellationToken);

            foreach (var id in subscriptionIds)
            {
                try
                {
                    // Load ONE aggregate at a time
                    var subscription = await aggregateContext.GetSubscriptionByIdAsync(id, cancellationToken);

                    if (subscription is null)
                        continue;

                    // Domain logic handles invoice creation rules
                    subscription.GenerateInvoice(today);
                }
                catch
                {
                    // Prevent one failure from stopping the entire batch
                    // (logging can be added here)
                }
            }

            // Save all generated invoices and updates
            await aggregateContext.SaveChangesAsync(cancellationToken);
        }
    }
}