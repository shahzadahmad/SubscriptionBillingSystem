using MediatR;
using SubscriptionBillingSystem.Application.Common.Interfaces;

namespace SubscriptionBillingSystem.Application.Common.Behaviors
{
    /// <summary>
    /// Transaction Behavior for MediatR Pipeline
    /// 
    /// Responsibilities:
    /// - Wraps request handling inside a database transaction
    /// - Ensures atomicity (all operations succeed or fail together)
    /// - Commits transaction if handler succeeds
    /// - Rolls back transaction if any exception occurs
    /// 
    /// This behavior is typically used for:
    /// - Commands (write operations)
    /// - Ensuring consistency across multiple repository operations
    /// 
    /// Benefits:
    /// - Prevents partial data updates
    /// - Centralizes transaction handling (no need in every handler)
    /// - Keeps application logic clean and focused
    /// </summary>
    /// <typeparam name="TRequest">Type of request (Command/Query)</typeparam>
    /// <typeparam name="TResponse">Type of response</typeparam>
    public class TransactionBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
    {
        /// <summary>
        /// Unit of Work abstraction to manage transactions and persistence
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor for injecting UnitOfWork dependency
        /// </summary>
        /// <param name="unitOfWork">Unit of Work instance</param>
        public TransactionBehavior(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Handles the pipeline execution within a database transaction
        /// 
        /// Flow:
        /// 1. Begin transaction
        /// 2. Execute the next pipeline step (handler)
        /// 3. Commit transaction if successful
        /// 4. Rollback transaction if an exception occurs
        /// 
        /// This ensures that all database operations performed
        /// within the handler are executed atomically.
        /// </summary>
        /// <param name="request">Incoming request (command/query)</param>
        /// <param name="next">Delegate to invoke the next pipeline step (handler)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Response from handler</returns>
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // Begin a new database transaction
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // Execute the actual request handler (business logic)
                var response = await next();

                // Commit transaction if everything succeeds
                await _unitOfWork.CommitAsync(cancellationToken);

                return response;
            }
            catch
            {
                // Rollback transaction in case of any failure
                // Ensures no partial data is persisted
                await _unitOfWork.RollbackAsync(cancellationToken);

                // Re-throw the exception so it can be handled by higher layers
                // (e.g., ExceptionHandlingMiddleware)
                throw;
            }
        }
    }
}