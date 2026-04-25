using MediatR;
using Microsoft.Extensions.Logging;

namespace SubscriptionBillingSystem.Application.Common.Behaviors
{
    /// <summary>
    /// Logging Behavior for MediatR Pipeline
    /// 
    /// Responsibilities:
    /// - Logs incoming requests before they are handled
    /// - Logs completion after the request is processed
    /// - Provides visibility into application flow (CQRS requests/commands/queries)
    /// 
    /// This is part of MediatR Pipeline Behaviors, which act like middleware
    /// but specifically for application layer request handling.
    /// 
    /// Benefits:
    /// - Centralized logging (no need to log in every handler)
    /// - Helps in debugging and tracing request execution
    /// - Improves observability in production systems
    /// </summary>
    /// <typeparam name="TRequest">Type of request (Command/Query)</typeparam>
    /// <typeparam name="TResponse">Type of response</typeparam>
    public class LoggingBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
    {
        /// <summary>
        /// Logger instance for writing structured logs
        /// </summary>
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        /// <summary>
        /// Constructor for injecting logger dependency
        /// </summary>
        /// <param name="logger">ILogger instance</param>
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Handles the pipeline execution
        /// 
        /// Flow:
        /// 1. Log incoming request (before handler execution)
        /// 2. Invoke the next delegate (actual handler)
        /// 3. Log after handler completes
        /// 
        /// This method wraps around the actual request handler.
        /// </summary>
        /// <param name="request">Incoming request (command/query)</param>
        /// <param name="next">Delegate to invoke the next pipeline step (or handler)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Response from handler</returns>
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // Log before handling the request
            // Using structured logging for better observability
            _logger.LogInformation("Handling {Request}", typeof(TRequest).Name);

            // Invoke the next behavior/handler in the pipeline
            var response = await next();

            // Log after the request has been successfully handled
            _logger.LogInformation("Handled {Request}", typeof(TRequest).Name);

            // Return the response back to the caller
            return response;
        }
    }
}