using MediatR;
using SubscriptionBillingSystem.Application.Common.Exceptions;
using SubscriptionBillingSystem.Application.Common.Interfaces;
using SubscriptionBillingSystem.Application.Common.Interfaces.Persistence;

namespace SubscriptionBillingSystem.Application.Features.Subscriptions.Commands
{
    /// <summary>
    /// Handles invoice generation request for a subscription.
    /// 
    /// Flow:
    /// 1. Load Subscription aggregate
    /// 2. Execute domain logic (GenerateInvoice)
    /// 3. TransactionBehavior will:
    ///    - Save changes
    ///    - Persist Outbox messages
    ///    - Commit transaction
    /// </summary>
    public class GenerateInvoiceHandler
        : IRequestHandler<GenerateInvoiceCommand, Unit>
    {
        private readonly IAggregateContext _context;
        private readonly IDateTime _dateTime;

        public GenerateInvoiceHandler(
            IAggregateContext context,
            IDateTime dateTime)
        {
            _context = context;
            _dateTime = dateTime;
        }

        public async Task<Unit> Handle(
            GenerateInvoiceCommand request,
            CancellationToken cancellationToken)
        {
            // Fetch Subscription aggregate
            var subscription = await _context.GetSubscriptionByIdAsync(request.SubscriptionId, cancellationToken);

            if (subscription is null)
                throw new NotFoundException(nameof(GenerateInvoiceCommand), request.SubscriptionId);

            // Execute domain logic (raises domain event only)
            subscription.GenerateInvoice(_dateTime.UtcNow);

            // ❌ DO NOT call SaveChangesAsync here
            // ✔ TransactionBehavior will handle:
            //    - SaveChanges
            //    - Outbox persistence
            //    - Transaction commit

            return Unit.Value;
        }
    }
}