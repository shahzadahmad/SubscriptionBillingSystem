using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SubscriptionBillingSystem.Application.Common.Interfaces;
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

            var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            var today = DateTime.UtcNow.Date;

            // Get all active subscriptions
            var subscriptions = await db.Subscriptions
                .Where(x => x.Status == SubscriptionStatus.Active)
                .ToListAsync(cancellationToken);

            foreach (var subscription in subscriptions)
            {
                try
                {
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
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}