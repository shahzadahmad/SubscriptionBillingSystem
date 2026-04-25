using FluentAssertions;
using SubscriptionBillingSystem.Domain.Aggregates.InvoiceAggregate;
using SubscriptionBillingSystem.Domain.Aggregates.SubscriptionAggregate;
using SubscriptionBillingSystem.Domain.Events;
using SubscriptionBillingSystem.Domain.Tests.Common;
using SubscriptionBillingSystem.Domain.ValueObjects;

namespace SubscriptionBillingSystem.Tests.Events
{
    public class DomainEventsTests : DomainTestBase
    {
        [Fact]
        public void ActivateSubscription_ShouldRaiseSubscriptionActivatedEvent()
        {
            var subscription = new Subscription(
                Guid.NewGuid(),
                new Money(50, "USD")
            );

            subscription.Activate(DateTime.UtcNow);

            HasEvent<SubscriptionActivatedEvent>(subscription)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ActivateSubscription_ShouldRaiseInvoiceGeneratedEvent()
        {
            var subscription = new Subscription(
                Guid.NewGuid(),
                new Money(100, "USD")
            );

            subscription.Activate(DateTime.UtcNow);

            var invoiceEvent = GetEvent<InvoiceGeneratedEvent>(subscription);

            invoiceEvent.Should().NotBeNull();

            var invoice = subscription.Invoices.Single();

            invoiceEvent!.InvoiceId.Should().Be(invoice.Id);
            invoiceEvent.SubscriptionId.Should().Be(subscription.Id);
        }

        [Fact]
        public void GenerateInvoice_ShouldRaiseInvoiceGeneratedEvent()
        {
            var subscription = new Subscription(
                Guid.NewGuid(),
                new Money(100, "USD")
            );

            var now = DateTime.UtcNow;

            subscription.Activate(now);

            subscription.ClearDomainEvents();

            subscription.GenerateInvoice(now.AddDays(31));

            HasEvent<InvoiceGeneratedEvent>(subscription)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void PayInvoice_ShouldRaisePaymentReceivedEvent()
        {
            var invoice = new Invoice(
                Guid.NewGuid(),
                new Money(200, "USD")
            );

            invoice.Pay();

            HasEvent<PaymentReceivedEvent>(invoice)
                .Should()
                .BeTrue();
        }
    }
}