using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SubscriptionBillingSystem.Application.Common.Behaviors;
using System.Reflection;

namespace SubscriptionBillingSystem.Application
{
    /// <summary>
    /// Application Layer Dependency Injection Configuration
    /// 
    /// Responsibilities:
    /// - Registers MediatR for CQRS handling (Commands/Queries)
    /// - Registers FluentValidation validators
    /// - Configures MediatR pipeline behaviors (cross-cutting concerns)
    /// 
    /// This method is called from API layer (Program.cs) to wire up
    /// all application-level dependencies in a clean and modular way.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers all Application layer services into the DI container
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Updated IServiceCollection</returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Get current assembly (Application layer)
            var assembly = Assembly.GetExecutingAssembly();

            #region MediatR Registration

            /// <summary>
            /// Registers MediatR and scans the assembly for:
            /// - IRequestHandler<TRequest, TResponse>
            /// - INotificationHandler<TNotification>
            /// 
            /// Enables CQRS pattern implementation.
            /// </summary>
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(assembly));

            #endregion

            #region FluentValidation Registration

            /// <summary>
            /// Registers all FluentValidation validators from the assembly
            /// 
            /// Example:
            /// - IValidator<CreateSubscriptionCommand>
            /// - IValidator<CreateCustomerCommand>
            /// 
            /// These validators are later used in ValidationBehavior.
            /// </summary>
            services.AddValidatorsFromAssembly(assembly);

            #endregion

            #region MediatR Pipeline Behaviors

            /// <summary>
            /// Registers pipeline behaviors (executed in order of registration)
            /// 
            /// Execution Order:
            /// 1. ValidationBehavior  → Validates request before processing
            /// 2. LoggingBehavior    → Logs request before & after execution
            /// 3. TransactionBehavior → Wraps handler in DB transaction
            /// 
            /// Important:
            /// Order matters — incorrect ordering can break logic.
            /// </summary>

            // 1. Validation (should be first to fail fast)
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // 2. Logging (wraps around handler execution)
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            // 3. Transaction (ensures atomic DB operations)
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            #endregion

            return services;
        }
    }
}