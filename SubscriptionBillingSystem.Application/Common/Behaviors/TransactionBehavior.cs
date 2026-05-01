using MediatR;
using SubscriptionBillingSystem.Application.Common.Interfaces;

namespace SubscriptionBillingSystem.Application.Common.Behaviors
{
    /// <summary>
    /// Transaction Behavior for MediatR Pipeline
    /// 
    /// Responsibilities:
    /// - Wraps ONLY Command requests inside a database transaction
    /// - Ensures atomicity (all operations succeed or fail together)
    /// - Commits transaction if handler succeeds
    /// - Rolls back transaction if any exception occurs
    /// 
    /// IMPORTANT:
    /// - This behavior is applied ONLY to Commands (write operations)
    /// - Queries (read operations) MUST NOT use transactions or SaveChanges
    /// 
    /// Benefits:
    /// - Prevents partial data updates
    /// - Enforces proper CQRS separation
    /// - Keeps application logic clean and focused
    /// </summary>
    public class TransactionBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionBehavior(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // Only apply transaction to Commands
            if (request is not ICommand)
            {
                return await next();
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var response = await next();

                // Commit only for Commands
                await _unitOfWork.CommitAsync(cancellationToken);

                return response;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}