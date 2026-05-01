using SubscriptionBillingSystem.Domain.Common;
using System.Text.Json.Serialization;

namespace SubscriptionBillingSystem.Domain.Events
{
    /// <summary>
    /// Raised when a subscription is successfully activated.
    /// </summary>
    public class SubscriptionActivatedEvent : DomainEvent
    {
        public Guid SubscriptionId { get; init; }

        /// <summary>
        /// Required for JSON deserialization (Outbox replay)
        /// </summary>
        public SubscriptionActivatedEvent() { }

        /// <summary>
        /// Used by domain logic
        /// </summary>
        [JsonConstructor]
        public SubscriptionActivatedEvent(Guid subscriptionId)
        {
            SubscriptionId = subscriptionId;            
        }

        public override void Validate()
        {
            if (SubscriptionId == Guid.Empty)
                throw new Exception("SubscriptionActivatedEvent: SubscriptionId is empty");            
        }
    }
}
