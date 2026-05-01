using SubscriptionBillingSystem.Domain.Common;
using System.Text.Json.Serialization;

namespace SubscriptionBillingSystem.Domain.Events
{
    /// <summary>
    /// Raised when an invoice payment is successfully received.
    /// </summary>
    public class PaymentReceivedEvent : DomainEvent
    {        
        public Guid InvoiceId { get; init; }        

        /// <summary>
        /// Required for JSON deserialization (Outbox replay)
        /// </summary>
        public PaymentReceivedEvent() { }

        /// <summary>
        /// Used by domain logic
        /// </summary>        
        [JsonConstructor]
        public PaymentReceivedEvent(Guid invoiceId)
        {
            InvoiceId = invoiceId;            
        }

        public override void Validate()
        {
            if (InvoiceId == Guid.Empty)
                throw new Exception("PaymentReceivedEvent: InvoiceId is empty");
        }
    }
}