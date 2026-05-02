using MediatR;
using SubscriptionBillingSystem.Application.Common.Exceptions;
using SubscriptionBillingSystem.Application.Common.Interfaces.Persistence;

namespace SubscriptionBillingSystem.Application.Features.Invoices.Commands
{
    /// <summary>
    /// Handles invoice payment.
    /// 
    /// Flow:
    /// 1. Load Invoice aggregate
    /// 2. Execute domain logic (Pay)
    /// 3. TransactionBehavior will:
    ///    - Save changes
    ///    - Persist Outbox messages
    ///    - Commit transaction
    /// </summary>
    public class PayInvoiceHandler : IRequestHandler<PayInvoiceCommand, Unit>
    {
        private readonly IAggregateContext _context;

        public PayInvoiceHandler(IAggregateContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle( 
            PayInvoiceCommand request,
            CancellationToken cancellationToken)
        {
            // Fetch Invoice aggregate
            var invoice = await _context.GetInvoiceByIdAsync(request.InvoiceId, cancellationToken);

            if (invoice is null)
                throw new NotFoundException(nameof(PayInvoiceCommand), request.InvoiceId);

            // Execute domain logic (raises PaymentReceivedEvent)
            invoice.Pay();

            // ❌ DO NOT call SaveChangesAsync here
            // ✔ TransactionBehavior will handle:
            //    - SaveChanges
            //    - Outbox persistence
            //    - Transaction commit

            return Unit.Value;
        }
    }
}