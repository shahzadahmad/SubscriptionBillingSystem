using MediatR;
using SubscriptionBillingSystem.Domain.Events;

namespace SubscriptionBillingSystem.Application.Features.Subscriptions.EventHandlers
{
    public class SubscriptionActivatedHandler
        : INotificationHandler<SubscriptionActivatedEvent>
    {
        public Task Handle(SubscriptionActivatedEvent notification, CancellationToken cancellationToken)
        {
            // Example actions:
            // - send email
            // - log activation
            // - trigger external system

            Console.WriteLine($"Subscription activated: {notification.SubscriptionId}");

            return Task.CompletedTask;
        }
    }
}