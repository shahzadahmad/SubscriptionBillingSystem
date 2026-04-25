using SubscriptionBillingSystem.Domain.Exceptions;

namespace SubscriptionBillingSystem.Domain.Aggregates.InvoiceAggregate
{
    public static class InvoiceErrors
    {
        public static Exception AlreadyPaid() =>
            new BusinessRuleViolationException("Invoice cannot be paid twice.");

        public static Exception InvalidAmount() =>
            new BusinessRuleViolationException("Invoice amount is invalid.");

        public static Exception NotFound() =>
            new BusinessRuleViolationException("Invoice not found.");

        public static Exception InvalidSubscription() =>
            new BusinessRuleViolationException("Invalid subscription for invoice.");
    }
}