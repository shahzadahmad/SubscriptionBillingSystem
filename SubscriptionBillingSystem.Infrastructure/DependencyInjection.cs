using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SubscriptionBillingSystem.Application.Common.Interfaces;
using SubscriptionBillingSystem.Infrastructure.BackgroundJobs;
using SubscriptionBillingSystem.Infrastructure.Persistence;
using SubscriptionBillingSystem.Infrastructure.Services;

namespace SubscriptionBillingSystem.Infrastructure
{
    /// <summary>
    /// Registers all infrastructure services:
    /// - EF Core DbContext
    /// - Unit of Work
    /// - Application abstractions (DateTime, etc.)
    /// - Outbox processing
    /// - Background jobs
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ======================================================
            // DATABASE (EF Core - InMemory for development/testing)
            // ======================================================
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("SubscriptionBillingDb");                

                options.ConfigureWarnings(w =>
                    w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            /*
             * Map Application abstraction to EF DbContext
             * This allows Application layer to depend on interface only
             */
            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<ApplicationDbContext>());

            // ======================================================
            // UNIT OF WORK (Transaction Management)
            // ======================================================
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ======================================================
            // SYSTEM SERVICES (Abstractions)
            // ======================================================
            /*
             * Provides current UTC time (used for testability)
             */
            services.AddScoped<IDateTime, DateTimeService>();

            // ======================================================
            // OUTBOX PATTERN (Event processing)
            // ======================================================
            services.AddScoped<OutboxProcessor>();

            /*
             * Background service continuously processes outbox messages
             */
            services.AddHostedService<OutboxBackgroundService>();

            // ======================================================
            // DOMAIN EVENTS DISPATCHER
            // ======================================================
            //services.AddScoped<DomainEventDispatcher>();

            return services;
        }
    }
}