using SubscriptionBillingSystem.Domain.Common;
using System.Text.Json.Serialization;

namespace SubscriptionBillingSystem.Domain.ValueObjects;

public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money() { } // EF

    [JsonConstructor]
    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw MoneyErrors.NegativeAmount();

        if (string.IsNullOrWhiteSpace(currency))
            throw MoneyErrors.InvalidCurrency();

        Amount = amount;
        Currency = currency;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}