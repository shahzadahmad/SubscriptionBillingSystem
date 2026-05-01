using MediatR;
using SubscriptionBillingSystem.Application.Common.Interfaces;

namespace SubscriptionBillingSystem.Application.Features.Subscriptions.Commands
{
    public class GenerateInvoiceCommand : ICommand<Unit>
    {
        public Guid SubscriptionId { get; set; }        
    }
}