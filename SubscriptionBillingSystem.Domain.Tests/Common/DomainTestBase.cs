using SubscriptionBillingSystem.Domain.Common;

namespace SubscriptionBillingSystem.Domain.Tests.Common
{
    public abstract class DomainTestBase
    {
        protected static T GetEvent<T>(AggregateRoot aggregate)
            where T : IDomainEvent
        {
            return aggregate.DomainEvents
                .OfType<T>()
                .SingleOrDefault();
        }

        protected static bool HasEvent<T>(AggregateRoot aggregate)
            where T : IDomainEvent
        {
            return aggregate.DomainEvents.Any(e => e is T);
        }
    }
}
