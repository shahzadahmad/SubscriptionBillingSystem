using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionBillingSystem.Application.Common.Exceptions;
using SubscriptionBillingSystem.Application.Common.Interfaces;

namespace SubscriptionBillingSystem.Application.Features.Subscriptions.Commands
{
    public class GenerateInvoiceHandler : IRequestHandler<GenerateInvoiceCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDateTime _dateTime;

        public GenerateInvoiceHandler(IApplicationDbContext context, IDateTime dateTime)
        {
            _context = context;
            _dateTime = dateTime;
        }

        public async Task Handle(GenerateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(x => x.Id == request.SubscriptionId, cancellationToken);

            if (subscription is null)
                throw new NotFoundException(nameof(subscription), request.SubscriptionId);

            subscription.GenerateInvoice(_dateTime.UtcNow);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}