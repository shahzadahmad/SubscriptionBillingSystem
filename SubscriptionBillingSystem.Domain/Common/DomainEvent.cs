namespace SubscriptionBillingSystem.Domain.Common
{
    public abstract class DomainEvent : IDomainEvent, IValidatableEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

        public abstract void Validate();
    }
}