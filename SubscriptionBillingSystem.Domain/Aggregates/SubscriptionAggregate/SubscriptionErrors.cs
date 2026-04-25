using SubscriptionBillingSystem.Domain.Exceptions;

namespace SubscriptionBillingSystem.Domain.Aggregates.SubscriptionAggregate
{
    public static class SubscriptionErrors
    {
        public static Exception NotActive() =>
            new BusinessRuleViolationException("Subscription is not active.");

        public static Exception AlreadyActive() =>
            new BusinessRuleViolationException("Subscription already active or cancelled.");

        public static Exception AlreadyCancelled() =>
            new BusinessRuleViolationException("Subscription already cancelled.");

        public static Exception InvoiceAlreadyGenerated() =>
            new BusinessRuleViolationException("Invoice already generated for this billing cycle.");

        public static Exception InvalidCustomerId() =>
            new BusinessRuleViolationException("Invalid customer id.");

        public static Exception InvalidBillingCycle() =>
            new BusinessRuleViolationException("Invalid billing cycle operation.");
    }
}