using FluentAssertions;
using SubscriptionBillingSystem.Domain.Aggregates.SubscriptionAggregate;
using SubscriptionBillingSystem.Domain.Events;
using SubscriptionBillingSystem.Domain.Tests.Common;
using SubscriptionBillingSystem.Domain.ValueObjects;

namespace SubscriptionBillingSystem.Domain.Tests.Aggregates
{
    public class SubscriptionTests : DomainTestBase
    {
        [Fact]
        public void Activate_ShouldSetActiveAndRaiseEvent()
        {
            var subscription = new Subscription(
                Guid.NewGuid(),
                new Money(50, "USD")
            );

            subscription.Activate(DateTime.UtcNow);

            subscription.Status.Should().Be(SubscriptionStatus.Active);

            HasEvent<SubscriptionActivatedEvent>(subscription)
                .Should()
                .BeTrue();

            subscription.Invoices.Should().HaveCount(1);
        }

        [Fact]
        public void Activate_AlreadyActive_ShouldThrow()
        {
            var subscription = new Subscription(
                Guid.NewGuid(),
                new Money(50, "USD")
            );

            subscription.Activate(DateTime.UtcNow);

            Action act = () => subscription.Activate(DateTime.UtcNow);

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Cancel_ShouldSetCancelled()
        {
            var subscription = new Subscription(
                Guid.NewGuid(),
                new Money(50, "USD")
            );

            subscription.Activate(DateTime.UtcNow);
            subscription.Cancel();

            subscription.Status.Should().Be(SubscriptionStatus.Cancelled);
        }

        [Fact]
        public void GenerateInvoice_WhenNotActive_ShouldThrow()
        {
            var subscription = new Subscription(
                Guid.NewGuid(),
                new Money(50, "USD")
            );

            Action act = () =>
                subscription.GenerateInvoice(DateTime.UtcNow);

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void GenerateInvoice_BeforeCycle_ShouldThrow()
        {
            var subscription = new Subscription(
                Guid.NewGuid(),
                new Money(50, "USD")
            );

            var now = DateTime.UtcNow;

            subscription.Activate(now);

            Action act = () =>
                subscription.GenerateInvoice(now.AddDays(10));

            act.Should().Throw<Exception>();
        }
    }
}
