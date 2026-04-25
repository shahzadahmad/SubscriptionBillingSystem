using SubscriptionBillingSystem.Domain.Aggregates.SubscriptionAggregate;
using SubscriptionBillingSystem.Domain.Common;
using SubscriptionBillingSystem.Domain.ValueObjects;

namespace SubscriptionBillingSystem.Domain.Aggregates.CustomerAggregate
{
    public class Customer : AggregateRoot
    {
        private readonly List<Subscription> _subscriptions = new();

        public string Name { get; private set; }
        public Email Email { get; private set; }

        public IReadOnlyCollection<Subscription> Subscriptions => _subscriptions;

        private Customer() { } // Required by EF Core

        public Customer(string name, Email email)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw CustomerErrors.NameRequired();

            Id = Guid.NewGuid();
            Name = name;
            Email = email;
        }
    }
}
