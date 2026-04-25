using MediatR;

namespace SubscriptionBillingSystem.Application.Features.Invoices.Commands
{
    public class PayInvoiceCommand : IRequest
    {
        public Guid InvoiceId { get; set; }
    }
}