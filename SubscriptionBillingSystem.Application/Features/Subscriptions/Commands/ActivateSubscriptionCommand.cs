using MediatR;

namespace SubscriptionBillingSystem.Application.Features.Subscriptions.Commands
{
    public class ActivateSubscriptionCommand : IRequest
    {
        public Guid SubscriptionId { get; set; }        
    }
}