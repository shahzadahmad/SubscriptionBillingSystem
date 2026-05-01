using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionBillingSystem.Application.Common.Exceptions;
using SubscriptionBillingSystem.Application.Common.Interfaces;

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
        private readonly IApplicationDbContext _context;
        private readonly IDateTime _dateTime;

        public ActivateSubscriptionHandler(
            IApplicationDbContext context,
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
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(
                    x => x.Id == request.SubscriptionId,
                    cancellationToken);

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