using FluentAssertions;
using SubscriptionBillingSystem.Domain.ValueObjects;

namespace SubscriptionBillingSystem.Domain.Tests.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void Create_ValidEmail_ShouldSucceed()
        {
            var email = new Email("test@gmail.com");

            email.Value.Should().Be("test@gmail.com");
        }

        [Fact]
        public void Create_InvalidEmail_ShouldThrow()
        {
            Action act = () => new Email("invalid-email");

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Create_EmptyEmail_ShouldThrow()
        {
            Action act = () => new Email("");

            act.Should().Throw<Exception>();
        }

    }
}
