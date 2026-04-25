using SubscriptionBillingSystem.Domain.Common;

namespace SubscriptionBillingSystem.Domain.Events
{
    public class InvoiceGeneratedEvent : IDomainEvent
    {
        public Guid InvoiceId { get; }
        public Guid SubscriptionId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public InvoiceGeneratedEvent(Guid invoiceId, Guid subscriptionId)
        {
            InvoiceId = invoiceId;
            SubscriptionId = subscriptionId;
        }
    }
}