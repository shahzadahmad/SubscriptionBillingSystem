using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionBillingSystem.Application.Common.Exceptions;
using SubscriptionBillingSystem.Application.Common.Interfaces;
using SubscriptionBillingSystem.Domain.Aggregates.InvoiceAggregate;

namespace SubscriptionBillingSystem.Application.Features.Subscriptions.Commands
{
    public class ActivateSubscriptionHandler : IRequestHandler<ActivateSubscriptionCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDateTime _dateTime;

        public ActivateSubscriptionHandler(
            IApplicationDbContext context,
            IDateTime dateTime)
        {
            _context = context;
            _dateTime = dateTime;
        }

        public async Task Handle(ActivateSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(x => x.Id == request.SubscriptionId, cancellationToken);

            if (subscription is null)
                throw new NotFoundException(nameof(subscription), request.SubscriptionId);

            // Use system-controlled time
            subscription.Activate(_dateTime.UtcNow);            

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}