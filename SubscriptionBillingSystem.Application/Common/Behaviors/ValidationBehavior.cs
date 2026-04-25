using FluentValidation;
using MediatR;

namespace SubscriptionBillingSystem.Application.Common.Behaviors
{
    /// <summary>
    /// Validation Behavior for MediatR Pipeline
    /// 
    /// Responsibilities:
    /// - Executes all registered FluentValidation validators for a request
    /// - Validates incoming commands/queries before they reach the handler
    /// - Prevents invalid data from entering business logic
    /// 
    /// This behavior acts as a guard layer in the application pipeline.
    /// 
    /// Benefits:
    /// - Centralized validation (no need to validate in every handler)
    /// - Keeps handlers clean and focused on business logic
    /// - Ensures consistency in validation across the application
    /// </summary>
    /// <typeparam name="TRequest">Type of request (Command/Query)</typeparam>
    /// <typeparam name="TResponse">Type of response</typeparam>
    public class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
    {
        /// <summary>
        /// Collection of validators for the given request type
        /// 
        /// Note:
        /// - Multiple validators can exist for a single request
        /// - All validators will be executed
        /// </summary>
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        /// <summary>
        /// Constructor for injecting validators
        /// </summary>
        /// <param name="validators">All registered validators for TRequest</param>
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        /// <summary>
        /// Handles validation before the request reaches the handler
        /// 
        /// Flow:
        /// 1. Check if any validators are registered
        /// 2. Create validation context using the request
        /// 3. Execute all validators
        /// 4. Collect all validation failures
        /// 5. Throw ValidationException if any errors exist
        /// 6. Otherwise, proceed to next pipeline step (handler)
        /// </summary>
        /// <param name="request">Incoming request (command/query)</param>
        /// <param name="next">Delegate to invoke next pipeline step</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Response from handler if validation passes</returns>
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // Only run validation if validators are registered for this request
            if (_validators.Any())
            {
                // Create validation context containing the request object
                var context = new ValidationContext<TRequest>(request);

                // Execute all validators and collect validation failures
                var failures = _validators
                    .Select(v => v.Validate(context)) // Run each validator
                    .SelectMany(r => r.Errors)       // Flatten all validation errors
                    .Where(f => f != null)           // Filter out null entries
                    .ToList();

                // If any validation errors exist, throw exception
                // This will be caught by ExceptionHandlingMiddleware
                if (failures.Count != 0)
                    throw new ValidationException(failures);
            }

            // Proceed to the next behavior or actual handler if validation passes
            return await next();
        }
    }
}