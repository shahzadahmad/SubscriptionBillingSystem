using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionBillingSystem.Application.Common.Exceptions;
using SubscriptionBillingSystem.Application.Common.Interfaces;

namespace SubscriptionBillingSystem.Application.Features.Subscriptions.Commands
{
    public class CancelSubscriptionHandler : IRequestHandler<CancelSubscriptionCommand>
    {
        private readonly IApplicationDbContext _context;

        public CancelSubscriptionHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(x => x.Id == request.SubscriptionId, cancellationToken);

            if (subscription is null)
                throw new NotFoundException(nameof(subscription), request.SubscriptionId);

            subscription.Cancel();

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}