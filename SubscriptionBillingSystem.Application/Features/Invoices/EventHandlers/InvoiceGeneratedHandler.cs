using MediatR;
using SubscriptionBillingSystem.Domain.Events;

namespace SubscriptionBillingSystem.Application.Features.Invoices.EventHandlers
{
    public class InvoiceGeneratedHandler
        : INotificationHandler<InvoiceGeneratedEvent>
    {
        public Task Handle(InvoiceGeneratedEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Invoice generated: {notification.InvoiceId}");

            return Task.CompletedTask;
        }
    }
}