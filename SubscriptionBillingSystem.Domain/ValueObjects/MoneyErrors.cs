using SubscriptionBillingSystem.Domain.Exceptions;

namespace SubscriptionBillingSystem.Domain.ValueObjects
{
    public static class MoneyErrors
    {
        public static Exception NegativeAmount() =>
            new BusinessRuleViolationException("Amount cannot be negative.");

        public static Exception InvalidCurrency() =>
            new BusinessRuleViolationException("Currency is required or invalid.");
    }
}