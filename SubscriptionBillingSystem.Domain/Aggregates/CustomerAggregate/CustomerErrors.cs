using SubscriptionBillingSystem.Domain.Exceptions;

namespace SubscriptionBillingSystem.Domain.Aggregates.CustomerAggregate
{
    public static class CustomerErrors
    {
        public static Exception NameRequired() =>
            new BusinessRuleViolationException("Customer name is required.");

        public static Exception EmailRequired() =>
            new BusinessRuleViolationException("Customer email is required.");

        public static Exception InvalidEmail() =>
            new BusinessRuleViolationException("Customer email format is invalid.");

        public static Exception InvalidCustomerId() =>
            new BusinessRuleViolationException("Customer id is invalid.");
    }
}