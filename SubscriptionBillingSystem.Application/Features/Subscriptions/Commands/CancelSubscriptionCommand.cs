using MediatR;

namespace SubscriptionBillingSystem.Application.Features.Subscriptions.Commands  
{
    public class CancelSubscriptionCommand : IRequest
    {
        public Guid SubscriptionId { get; set; }
    }
}