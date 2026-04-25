using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionBillingSystem.Domain.Aggregates.SubscriptionAggregate;
using SubscriptionBillingSystem.Domain.Aggregates.InvoiceAggregate;

namespace SubscriptionBillingSystem.Infrastructure.Persistence.Configurations
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.LastBillingDate);

            // Value Object
            builder.OwnsOne(x => x.MonthlyPrice, money =>
            {
                money.Property(m => m.Amount).IsRequired();
                money.Property(m => m.Currency).HasMaxLength(3).IsRequired();
            });

            // ONLY RELATIONSHIP DEFINED HERE
            builder.HasMany(x => x.Invoices)
                .WithOne(i => i.Subscription)
                .HasForeignKey(i => i.SubscriptionId);                

            // USE BACKING FIELD
            builder.Metadata
                .FindNavigation(nameof(Subscription.Invoices))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}