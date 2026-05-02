using SubscriptionBillingSystem.Application.Features.Invoices.Models;
using SubscriptionBillingSystem.Domain.Aggregates.InvoiceAggregate;
using SubscriptionBillingSystem.Domain.Aggregates.SubscriptionAggregate;

namespace SubscriptionBillingSystem.Application.Common.Interfaces.Persistence
{
    public interface IReadDbContext
    {
        Task<List<GetInvoicesDto>> GetInvoicesAsync(Guid? subscriptionId, CancellationToken cancellationToken);
        Task<List<Guid>> GetActiveSubscriptionIdsAsync(CancellationToken cancellationToken);
    }
}