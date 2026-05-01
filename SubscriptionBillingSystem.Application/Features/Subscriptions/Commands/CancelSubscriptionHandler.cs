using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionBillingSystem.Application.Common.Exceptions;
using SubscriptionBillingSystem.Application.Common.Interfaces;

namespace SubscriptionBillingSystem.Application.Features.Subscriptions.Commands
{
    /// <summary>
    /// Handles subscription cancellation.
    /// 
    /// Flow:
    /// 1. Load Subscription aggregate
    /// 2. Execute domain logic (Cancel)
    /// 3. TransactionBehavior will persist changes + Outbox
    /// </summary>
    public class CancelSubscriptionHandler
        : IRequestHandler<CancelSubscriptionCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public CancelSubscriptionHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(
            CancelSubscriptionCommand request,
            CancellationToken cancellationToken)
        {
            // Fetch Subscription aggregate
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(
                    x => x.Id == request.SubscriptionId,
                    cancellationToken);

            if (subscription is null)
                throw new NotFoundException(nameof(CancelSubscriptionCommand), request.SubscriptionId);

            // Execute domain logic (raises domain events if any)
            subscription.Cancel();

            // ❌ DO NOT call SaveChangesAsync here
            // ✔ TransactionBehavior will handle:
            //    - SaveChanges
            //    - Outbox persistence
            //    - Transaction commit

            return Unit.Value;
        }
    }
}