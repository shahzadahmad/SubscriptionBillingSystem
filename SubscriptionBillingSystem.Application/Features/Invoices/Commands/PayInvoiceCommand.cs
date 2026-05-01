using MediatR;
using SubscriptionBillingSystem.Application.Common.Interfaces;

namespace SubscriptionBillingSystem.Application.Features.Invoices.Commands
{
    public class PayInvoiceCommand : ICommand<Unit>
    {
        public Guid InvoiceId { get; set; }
    }
}