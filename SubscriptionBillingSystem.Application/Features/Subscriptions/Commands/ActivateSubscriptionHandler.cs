using MediatR;
using SubscriptionBillingSystem.Application.Common.Exceptions;
using SubscriptionBillingSystem.Application.Common.Interfaces;
using SubscriptionBillingSystem.Application.Common.Interfaces.Persistence;

namespace SubscriptionBillingSystem.Application.Features.Subscriptions.Commands
{
    /// <summary>
    /// Handles subscription activation.
    /// 
    /// Flow:
    /// 1. Load Subscription aggregate
    /// 2. Execute domain logic (Activate)
    /// 3. TransactionBehavior will persist changes + Outbox
    /// </summary>
    public class ActivateSubscriptionHandler
        : IRequestHandler<ActivateSubscriptionCommand, Unit>
    {
        private readonly IAggregateContext _context;
        private readonly IDateTime _dateTime;

        public ActivateSubscriptionHandler(
            IAggregateContext context,
            IDateTime dateTime)
        {
            _context = context;
            _dateTime = dateTime;
        }

        public async Task<Unit> Handle(
            ActivateSubscriptionCommand request,
            CancellationToken cancellationToken)
        {
            // Fetch Subscription aggregate            
            var subscription = await _context.GetSubscriptionByIdAsync(request.SubscriptionId, cancellationToken);

            if (subscription is null)
                throw new NotFoundException(nameof(ActivateSubscriptionCommand), request.SubscriptionId);

            // Execute domain logic (raises domain events)
            subscription.Activate(_dateTime.UtcNow);

            // ❌ DO NOT call SaveChangesAsync here
            // TransactionBehavior will handle:
            // - SaveChanges
            // - Outbox persistence
            // - Transaction commit

            return Unit.Value;
        }
    }
}