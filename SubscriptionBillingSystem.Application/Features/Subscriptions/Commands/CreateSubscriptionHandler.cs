using MediatR;
using SubscriptionBillingSystem.Application.Common.Interfaces;
using SubscriptionBillingSystem.Domain.Aggregates.SubscriptionAggregate;
using SubscriptionBillingSystem.Domain.ValueObjects;

namespace SubscriptionBillingSystem.Application.Features.Subscriptions.Commands
{
    /// <summary>
    /// Handles creation of a new Subscription.
    /// 
    /// Flow:
    /// 1. Create Subscription aggregate (Domain)
    /// 2. Add to DbContext (Tracking only)
    /// 3. TransactionBehavior will:
    ///    - Save changes
    ///    - Persist Outbox messages
    ///    - Commit transaction
    /// </summary>
    public class CreateSubscriptionHandler
        : IRequestHandler<CreateSubscriptionCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateSubscriptionHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(
            CreateSubscriptionCommand request,
            CancellationToken cancellationToken)
        {
            // Create Aggregate Root
            var subscription = new Subscription(
                request.CustomerId,
                new Money(request.MonthlyPrice, request.Currency)
            );

            // Add entity to DbContext (no DB call yet)
            await _context.Subscriptions.AddAsync(subscription, cancellationToken);

            // ❌ DO NOT call SaveChangesAsync here
            // ✔ TransactionBehavior will handle:
            //    - SaveChanges
            //    - Outbox persistence
            //    - Transaction commit

            return subscription.Id;
        }
    }
}