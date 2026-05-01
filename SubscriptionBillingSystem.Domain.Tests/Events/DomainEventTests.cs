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
        public void ActivateSubscription_ShouldRaiseInvoiceGenerationRequestedEvent()
        {
            var subscription = new Subscription(
                Guid.NewGuid(),
                new Money(100, "USD")
            );

            subscription.Activate(DateTime.UtcNow);

            // ✔ Correct event now raised (NOT InvoiceGeneratedEvent)
            var evt = GetEvent<InvoiceGenerationRequestedEvent>(subscription);

            evt.Should().NotBeNull();

            evt!.SubscriptionId.Should().Be(subscription.Id);
            evt.Amount.Should().Be(subscription.MonthlyPrice);            
        }

        [Fact]
        public void GenerateInvoice_ShouldRaiseInvoiceGenerationRequestedEvent()
        {
            var subscription = new Subscription(
                Guid.NewGuid(),
                new Money(100, "USD")
            );

            var now = DateTime.UtcNow;

            subscription.Activate(now);

            subscription.ClearDomainEvents();

            subscription.GenerateInvoice(now.AddDays(31));

            HasEvent<InvoiceGenerationRequestedEvent>(subscription)
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