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
        public Money MonthlyPrice { get; private set; }        
        public DateTime? LastBillingDate { get; private set; }

        private const int BillingCycleDays = 30;

        private readonly List<Invoice> _invoices = new();
        public IReadOnlyCollection<Invoice> Invoices => _invoices.AsReadOnly();

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

            RaiseDomainEvent(new SubscriptionActivatedEvent(Id));

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
        }

        // -------------------------
        // BUSINESS ACTION: BILLING
        // -------------------------
        public void GenerateInvoice(DateTime billingDate)
        {
            EnsureCanGenerateInvoice(billingDate);

            var invoice = new Invoice(Id, MonthlyPrice);

            _invoices.Add(invoice);
            LastBillingDate = billingDate;

            RaiseDomainEvent(new InvoiceGeneratedEvent(invoice.Id, invoice.SubscriptionId));
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