using MediatR;
using SubscriptionBillingSystem.Application.Common.Interfaces;
using SubscriptionBillingSystem.Domain.Aggregates.SubscriptionAggregate;
using SubscriptionBillingSystem.Domain.ValueObjects;

namespace SubscriptionBillingSystem.Application.Features.Subscriptions.Commands
{
    public class CreateSubscriptionHandler : IRequestHandler<CreateSubscriptionCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateSubscriptionHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var subscription = new Subscription(
                request.CustomerId,
                new Money(request.MonthlyPrice, request.Currency)
            );

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync(cancellationToken);

            return subscription.Id;
        }
    }
}