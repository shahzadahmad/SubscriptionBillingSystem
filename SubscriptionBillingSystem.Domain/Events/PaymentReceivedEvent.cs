using SubscriptionBillingSystem.Domain.Common;

namespace SubscriptionBillingSystem.Domain.Events
{
    public class PaymentReceivedEvent : IDomainEvent
    {
        public Guid InvoiceId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public PaymentReceivedEvent(Guid invoiceId)
        {
            InvoiceId = invoiceId;
        }
    }
}