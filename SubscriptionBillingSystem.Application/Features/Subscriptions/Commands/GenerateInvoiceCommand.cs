using MediatR;

namespace SubscriptionBillingSystem.Application.Features.Subscriptions.Commands
{
    public class GenerateInvoiceCommand : IRequest
    {
        public Guid SubscriptionId { get; set; }        
    }
}