using SubscriptionBillingSystem.Domain.Common;
using System.Text.Json.Serialization;

namespace SubscriptionBillingSystem.Domain.Events
{
    /// <summary>
    /// Raised after an invoice has been successfully generated.
    /// </summary>
    public class InvoiceGeneratedEvent : DomainEvent
    {
        public Guid InvoiceId { get; init; }
        public Guid SubscriptionId { get; init; }        

        /// <summary>
        /// Required for JSON deserialization (Outbox replay)
        /// </summary>
        public InvoiceGeneratedEvent() { }

        /// <summary>
        /// Used by domain/application layer
        /// </summary>
        [JsonConstructor]
        public InvoiceGeneratedEvent(Guid invoiceId, Guid subscriptionId)
        {
            InvoiceId = invoiceId;
            SubscriptionId = subscriptionId;
        }

        public override void Validate()
        {
            if (InvoiceId == Guid.Empty)
                throw new Exception("InvoiceGeneratedEvent: InvoiceId is empty");

            if (SubscriptionId == Guid.Empty)
                throw new Exception("InvoiceGeneratedEvent: SubscriptionId is empty");
        }
    }
}