using Microsoft.EntityFrameworkCore;
using SubscriptionBillingSystem.Application.Common.Interfaces.Persistence;
using SubscriptionBillingSystem.Application.Features.Invoices.Models;
using SubscriptionBillingSystem.Domain.Aggregates.SubscriptionAggregate;


namespace SubscriptionBillingSystem.Infrastructure.Persistence.ReadModels
{
    public class ReadDbContext : IReadDbContext
    {
        private readonly ApplicationDbContext _context;

        public ReadDbContext(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GetInvoicesDto>> GetInvoicesAsync(Guid? subscriptionId, CancellationToken cancellationToken)
        {
            var query = _context.Invoices.AsNoTracking();

            if (subscriptionId.HasValue)
            {
                query = query.Where(x => x.SubscriptionId == subscriptionId.Value);
            }

            return await query
                .Select(x => new GetInvoicesDto
                {
                    InvoiceId = x.Id,
                    SubscriptionId = x.SubscriptionId,
                    Amount = x.Amount!.Amount,
                    Currency = x.Amount!.Currency,
                    Status = x.Status.ToString(),
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Guid>> GetActiveSubscriptionIdsAsync(CancellationToken cancellationToken)
        {
            return await _context.Subscriptions
                .AsNoTracking()
                .Where(x => x.Status == SubscriptionStatus.Active)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);
        }
    }
}