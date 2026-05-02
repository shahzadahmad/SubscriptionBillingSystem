using Microsoft.EntityFrameworkCore;
using SubscriptionBillingSystem.Domain.Aggregates.CustomerAggregate;
using SubscriptionBillingSystem.Domain.Aggregates.InvoiceAggregate;
using SubscriptionBillingSystem.Domain.Aggregates.SubscriptionAggregate;

namespace SubscriptionBillingSystem.Application.Common.Interfaces.Persistence
{
    public interface IAggregateContext
    {
        DbSet<Customer> Customers { get; }
        DbSet<Subscription> Subscriptions { get; }
        DbSet<Invoice> Invoices { get; }        

        // Aggregate access
        Task<Invoice?> GetInvoiceByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Subscription?> GetSubscriptionByIdAsync(Guid id, CancellationToken cancellationToken);        

        // Persistence
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    }
}