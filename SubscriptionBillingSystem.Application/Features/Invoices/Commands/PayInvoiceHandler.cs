using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionBillingSystem.Application.Common.Exceptions;
using SubscriptionBillingSystem.Application.Common.Interfaces;
using SubscriptionBillingSystem.Domain.Aggregates.SubscriptionAggregate;

namespace SubscriptionBillingSystem.Application.Features.Invoices.Commands
{
    public class PayInvoiceHandler : IRequestHandler<PayInvoiceCommand>
    {
        private readonly IApplicationDbContext _context;

        public PayInvoiceHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(PayInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);

            if (invoice is null)
                throw new NotFoundException(nameof(invoice), request.InvoiceId);

            invoice.Pay();

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}