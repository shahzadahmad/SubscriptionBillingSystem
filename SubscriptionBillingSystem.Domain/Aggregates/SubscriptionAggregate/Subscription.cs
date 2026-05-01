using SubscriptionBillingSystem.Domain.Aggregates.InvoiceAggregate;
using SubscriptionBillingSystem.Domain.Common;
using SubscriptionBillingSystem.Domain.Events;
using SubscriptionBillingSystem.Domain.ValueObjects;

namespace SubscriptionBillingSystem.Domain.Aggregates.SubscriptionAggregate
{
    public class Subscription : AggregateRoot
    {
        public Guid CustomerId { get; private set; }
        public SubscriptionStatus Status { get; private set; }
        public Money? MonthlyPrice { get; private set; }        
        public DateTime? LastBillingDate { get; private set; }

        private const int BillingCycleDays = 30;

        private Subscription() { }

        public Subscription(Guid customerId, Money monthlyPrice)
        {
            if (customerId == Guid.Empty)
                throw SubscriptionErrors.InvalidCustomerId();

            Id = Guid.NewGuid();
            CustomerId = customerId;
            MonthlyPrice = monthlyPrice;            
            Status = SubscriptionStatus.Inactive;
        }

        // -------------------------
        // BUSINESS ACTION: ACTIVATE
        // -------------------------
        public void Activate(DateTime activationDate)
        {
            if (Status == SubscriptionStatus.Active)
                throw SubscriptionErrors.AlreadyActive();

            Status = SubscriptionStatus.Active;

            // Raise event instead of performing external actions directly
            RaiseDomainEvent(new SubscriptionActivatedEvent(Id));

            // Trigger billing via domain logic (event-driven)
            GenerateInvoice(activationDate);
        }

        // -------------------------
        // BUSINESS ACTION: CANCEL
        // -------------------------
        public void Cancel()
        {
            if (Status == SubscriptionStatus.Cancelled)
                throw SubscriptionErrors.AlreadyCancelled();

            Status = SubscriptionStatus.Cancelled;

            // NOTE:
            // We DO NOT delete invoices or modify history.
            // Future billing will stop due to status check.
        }

        // -------------------------
        // BUSINESS ACTION: BILLING
        // -------------------------
        public void GenerateInvoice(DateTime billingDate)
        {
            // Validate business rules before allowing billing
            EnsureCanGenerateInvoice(billingDate);

            // IMPORTANT:
            // We intentionally do NOT create an Invoice inside the aggregate.
            // This keeps Subscription and Invoice as independent aggregates
            // and prevents tight coupling in the domain model.

            LastBillingDate = billingDate;

            // DOMAIN DECISION:
            // We raise a domain event to indicate that invoice generation has been requested.
            // The actual Invoice creation is handled outside the aggregate
            // (Application Layer via event handler), ensuring proper separation of concerns.

            RaiseDomainEvent(new InvoiceGenerationRequestedEvent(
                subscriptionId: Id,
                amount: MonthlyPrice
            ));
        }

        // -------------------------
        // INTERNAL RULES
        // -------------------------
        private void EnsureCanGenerateInvoice(DateTime billingDate)
        {
            if (Status != SubscriptionStatus.Active)
                throw SubscriptionErrors.NotActive();

            if (LastBillingDate.HasValue &&
                (billingDate - LastBillingDate.Value).TotalDays < BillingCycleDays)
            {
                throw SubscriptionErrors.InvoiceAlreadyGenerated();
            }
        }
    }
}