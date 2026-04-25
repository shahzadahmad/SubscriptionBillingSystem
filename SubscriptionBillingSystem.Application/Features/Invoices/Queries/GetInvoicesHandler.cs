using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionBillingSystem.Application.Common.Interfaces;
using SubscriptionBillingSystem.Application.Features.Invoices.Models;

namespace SubscriptionBillingSystem.Application.Features.Invoices.Queries
{
    public class GetInvoicesHandler : IRequestHandler<GetInvoicesQuery, List<GetInvoicesDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetInvoicesHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GetInvoicesDto>> Handle(
            GetInvoicesQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.Invoices.AsNoTracking();

            if (request.SubscriptionId.HasValue)
            {
                query = query.Where(x => x.SubscriptionId == request.SubscriptionId.Value);
            }

            var result = await query
                .Select(x => new GetInvoicesDto
                {
                    InvoiceId = x.Id,
                    SubscriptionId = x.SubscriptionId,
                    Amount = x.Amount.Amount,
                    Currency = x.Amount.Currency,
                    Status = x.Status.ToString(),
                    CreatedAt = x.Id != Guid.Empty ? DateTime.UtcNow : DateTime.UtcNow // placeholder if no CreatedAt in domain
                })
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}
