using MediatR;
using SubscriptionBillingSystem.Application.Common.Interfaces.Persistence;
using SubscriptionBillingSystem.Domain.Aggregates.InvoiceAggregate;
using SubscriptionBillingSystem.Domain.Events;

namespace SubscriptionBillingSystem.Application.Features.Invoices.EventHandlers
{
    /// <summary>
    /// Handles invoice creation when a subscription requests billing.
    /// </summary>
    public class InvoiceGenerationRequestedHandler
        : INotificationHandler<InvoiceGenerationRequestedEvent>
    {
        private readonly IAggregateContext _context;
        private readonly IMediator _mediator;

        public InvoiceGenerationRequestedHandler(
            IAggregateContext context,
            IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task Handle(
            InvoiceGenerationRequestedEvent notification,
            CancellationToken cancellationToken)
        {
            // STEP 1: Create invoice (Application responsibility)
            var invoice = new Invoice(
                subscriptionId: notification.SubscriptionId,
                amount: notification.Amount 
            );

            // STEP 2: Persist invoice
            await _context.Invoices.AddAsync(invoice, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // STEP 3: Raise completion event AFTER successful persistence
            await _mediator.Publish(
                new InvoiceGeneratedEvent(invoice.Id, invoice.SubscriptionId),
                cancellationToken
            );
        }
    }
}