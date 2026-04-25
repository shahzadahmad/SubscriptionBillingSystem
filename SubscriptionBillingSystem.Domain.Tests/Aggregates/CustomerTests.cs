using FluentAssertions;
using SubscriptionBillingSystem.Domain.Aggregates.CustomerAggregate;
using SubscriptionBillingSystem.Domain.ValueObjects;

namespace SubscriptionBillingSystem.Domain.Tests.Aggregates
{
    public class CustomerTests
    {
        [Fact]
        public void Create_ValidCustomer_ShouldSucceed()
        {
            var customer = new Customer(
                "Shahzad",
                new Email("test@gmail.com")
            );

            customer.Name.Should().Be("Shahzad");
            customer.Email.Value.Should().Be("test@gmail.com");
        }

        [Fact]
        public void Create_EmptyName_ShouldThrow()
        {
            Action act = () =>
                new Customer("", new Email("test@gmail.com"));

            act.Should().Throw<Exception>();
        }
    }
}
