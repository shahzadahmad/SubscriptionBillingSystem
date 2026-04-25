using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SubscriptionBillingSystem.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Background service that periodically triggers Outbox processing.
    /// </summary>
    public class OutboxBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public OutboxBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Executes Outbox processing in a continuous loop until application shutdown.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var interval = TimeSpan.FromMinutes(5);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Create a scoped lifetime for resolving scoped services (like DbContext)
                    using var scope = _scopeFactory.CreateScope();

                    // Resolve Outbox processor from DI container
                    var processor = scope.ServiceProvider
                        .GetRequiredService<OutboxProcessor>();

                    // Execute Outbox processing
                    await processor.ProcessAsync();
                }
                catch
                {
                    // Prevent background service from crashing due to unexpected errors
                    // (logging should be added here in production)
                }

                // Wait before next execution cycle
                await Task.Delay(interval, stoppingToken);
            }
        }
    }
}