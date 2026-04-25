using SubscriptionBillingSystem.Domain.Common;

namespace SubscriptionBillingSystem.Domain.Events
{
    public class SubscriptionActivatedEvent : IDomainEvent
    {
        public Guid SubscriptionId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public SubscriptionActivatedEvent(Guid subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
    }
}