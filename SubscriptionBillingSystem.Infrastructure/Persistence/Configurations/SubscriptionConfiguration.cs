using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionBillingSystem.Domain.Aggregates.InvoiceAggregate;
using SubscriptionBillingSystem.Domain.Aggregates.SubscriptionAggregate;

namespace SubscriptionBillingSystem.Infrastructure.Persistence.Configurations
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            // PRIMARY KEY
            builder.HasKey(x => x.Id);

            // BASIC PROPERTIES
            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.LastBillingDate);

            // VALUE OBJECT: Money
            builder.OwnsOne(x => x.MonthlyPrice, money =>
            {
                money.Property(m => m.Amount).IsRequired();
                money.Property(m => m.Currency)
                    .HasMaxLength(3)
                    .IsRequired();
            });  
        }
    }
}