using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionBillingSystem.Application.Common.Interfaces;
using SubscriptionBillingSystem.Application.Features.Invoices.Models;

namespace SubscriptionBillingSystem.Application.Features.Invoices.Queries
{
    /// <summary>
    /// Handles retrieval of invoices (READ ONLY operation).
    /// 
    /// CQRS Rule:
    /// - No domain modification
    /// - No transactions
    /// - No SaveChanges
    /// - Optimized with AsNoTracking
    /// </summary>
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
            // Base query (read-only)
            var query = _context.Invoices.AsNoTracking();

            // Optional filter
            if (request.SubscriptionId.HasValue)
            {
                query = query.Where(x => x.SubscriptionId == request.SubscriptionId.Value);
            }

            // Projection to DTO
            var result = await query
                .Select(x => new GetInvoicesDto
                {
                    InvoiceId = x.Id,
                    SubscriptionId = x.SubscriptionId,
                    Amount = x.Amount.Amount,
                    Currency = x.Amount.Currency,
                    Status = x.Status.ToString(),                    
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}