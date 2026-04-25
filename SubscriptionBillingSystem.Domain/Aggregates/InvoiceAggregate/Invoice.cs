using SubscriptionBillingSystem.Domain.Aggregates.SubscriptionAggregate;
using SubscriptionBillingSystem.Domain.Common;
using SubscriptionBillingSystem.Domain.Events;
using SubscriptionBillingSystem.Domain.ValueObjects;

namespace SubscriptionBillingSystem.Domain.Aggregates.InvoiceAggregate
{
    public class Invoice : AggregateRoot
    {
        public Guid SubscriptionId { get; private set; }
        public Money Amount { get; private set; }
        public InvoiceStatus Status { get; private set; }

        // 🔥 Navigation property
        public Subscription Subscription { get; private set; } = null!;

        private Invoice() { } // EF Core

        public Invoice(Guid subscriptionId, Money amount)
        {
            Id = Guid.NewGuid();
            SubscriptionId = subscriptionId;
            Amount = amount;
            Status = InvoiceStatus.Pending;
        }

        public void Pay()
        {
            if (Status == InvoiceStatus.Paid)
                throw InvoiceErrors.AlreadyPaid();

            Status = InvoiceStatus.Paid;

            RaiseDomainEvent(new PaymentReceivedEvent(Id));
        }
    }
}