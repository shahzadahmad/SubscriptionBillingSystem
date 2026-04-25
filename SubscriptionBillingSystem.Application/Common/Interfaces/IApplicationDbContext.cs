using Microsoft.EntityFrameworkCore;
using SubscriptionBillingSystem.Domain.Aggregates.CustomerAggregate;
using SubscriptionBillingSystem.Domain.Aggregates.InvoiceAggregate;
using SubscriptionBillingSystem.Domain.Aggregates.SubscriptionAggregate;
using System.Collections.Generic;

namespace SubscriptionBillingSystem.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Customer> Customers { get; }
        DbSet<Subscription> Subscriptions { get; }
        DbSet<Invoice> Invoices { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}