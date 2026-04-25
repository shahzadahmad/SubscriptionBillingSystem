using FluentAssertions;
using SubscriptionBillingSystem.Domain.Aggregates.InvoiceAggregate;
using SubscriptionBillingSystem.Domain.ValueObjects;

namespace SubscriptionBillingSystem.Domain.Tests.Aggregates
{
    public class InvoiceTests
    {
        [Fact]
        public void Create_Invoice_ShouldBePending()
        {
            var invoice = new Invoice(
                Guid.NewGuid(),
                new Money(100, "USD")
            );

            invoice.Status.Should().Be(InvoiceStatus.Pending);
        }

        [Fact]
        public void Pay_Invoice_ShouldMarkAsPaid()
        {
            var invoice = new Invoice(
                Guid.NewGuid(),
                new Money(100, "USD")
            );

            invoice.Pay();

            invoice.Status.Should().Be(InvoiceStatus.Paid);
        }

        [Fact]
        public void Pay_AlreadyPaidInvoice_ShouldThrow()
        {
            var invoice = new Invoice(
                Guid.NewGuid(),
                new Money(100, "USD")
            );

            invoice.Pay();

            Action act = () => invoice.Pay();

            act.Should().Throw<Exception>();
        }
    }
}
