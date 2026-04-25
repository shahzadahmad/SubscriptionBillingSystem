using MediatR;

namespace SubscriptionBillingSystem.Domain.Common
{
    public interface IDomainEvent : INotification
    {
        DateTime OccurredOn { get; }
    }
}
