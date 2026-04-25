using FluentAssertions;
using SubscriptionBillingSystem.Domain.ValueObjects;

namespace SubscriptionBillingSystem.Domain.Tests.ValueObjects
{
    public class MoneyTests
    {
        [Fact]
        public void Create_ValidMoney_ShouldSucceed()
        {
            var money = new Money(100, "USD");

            money.Amount.Should().Be(100);
            money.Currency.Should().Be("USD");
        }

        [Fact]
        public void Create_NegativeAmount_ShouldThrow()
        {
            Action act = () => new Money(-10, "USD");

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Create_EmptyCurrency_ShouldThrow()
        {
            Action act = () => new Money(100, "");

            act.Should().Throw<Exception>();
        }
    }
}
