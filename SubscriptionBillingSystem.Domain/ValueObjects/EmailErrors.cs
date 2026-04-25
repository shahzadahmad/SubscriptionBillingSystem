using SubscriptionBillingSystem.Domain.Exceptions;

namespace SubscriptionBillingSystem.Domain.ValueObjects
{
    public static class EmailErrors
    {
        public static Exception Required() =>
            new BusinessRuleViolationException("Email is required.");

        public static Exception InvalidFormat() =>
            new BusinessRuleViolationException("Email format is invalid.");
    }
}