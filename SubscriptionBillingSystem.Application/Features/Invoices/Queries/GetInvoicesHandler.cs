using MediatR;
using SubscriptionBillingSystem.Application.Common.Interfaces.Persistence;
using SubscriptionBillingSystem.Application.Features.Invoices.Models;

namespace SubscriptionBillingSystem.Application.Features.Invoices.Queries
{
    public class GetInvoicesHandler
        : IRequestHandler<GetInvoicesQuery, List<GetInvoicesDto>>
    {
        private readonly IReadDbContext _readDb;

        public GetInvoicesHandler(IReadDbContext readDb)
        {
            _readDb = readDb;
        }

        public Task<List<GetInvoicesDto>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
        {
            return _readDb.GetInvoicesAsync(request.SubscriptionId, cancellationToken);
        }
    }
}