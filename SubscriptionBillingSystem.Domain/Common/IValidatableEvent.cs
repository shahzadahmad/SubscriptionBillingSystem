namespace SubscriptionBillingSystem.Domain.Common
{
    /// <summary>
    /// Contract for validating domain events after deserialization (Outbox replay safety)
    /// </summary>
    public interface IValidatableEvent
    {
        void Validate();
    }
}