using SubscriptionBillingSystem.Domain.Aggregates.InvoiceAggregate;
using SubscriptionBillingSystem.Domain.Common;
using SubscriptionBillingSystem.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace SubscriptionBillingSystem.Domain.Events
{
    /// <summary>
    /// Raised when a subscription requests invoice generation.
    /// Actual invoice creation happens in Application layer.
    /// </summary>
    public class InvoiceGenerationRequestedEvent : DomainEvent
    {
        public Guid SubscriptionId { get; init; }
        public Money? Amount { get; init; }

        /// <summary>
        /// Required for JSON deserialization (Outbox replay)
        /// </summary>
        public InvoiceGenerationRequestedEvent() { }

        /// <summary>
        /// Used by domain logic
        /// </summary>    
        [JsonConstructor]
        public InvoiceGenerationRequestedEvent(Guid subscriptionId, Money amount)
        {
            SubscriptionId = subscriptionId;
            Amount = amount;
        }

        public override void Validate()
        {
            if (SubscriptionId == Guid.Empty)
                throw new Exception("InvoiceGenerationRequestedEvent: SubscriptionId is empty");

            if (Amount is not null && Amount.Amount <= 0)
                throw new Exception("InvoiceGenerationRequestedEvent: Amount must be greater than zero");

            if (Amount is not null && string.IsNullOrWhiteSpace(Amount.Currency))
                throw new Exception("InvoiceGenerationRequestedEvent: Currency is required");
        }
    }
}
