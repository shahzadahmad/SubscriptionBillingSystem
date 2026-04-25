using MediatR;
using SubscriptionBillingSystem.Domain.Events;

namespace SubscriptionBillingSystem.Application.EventHandlers.Payments
{
    public class PaymentReceivedHandler
        : INotificationHandler<PaymentReceivedEvent>
    {
        public Task Handle(PaymentReceivedEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Payment received for invoice: {notification.InvoiceId}");

            return Task.CompletedTask;
        }
    }
}