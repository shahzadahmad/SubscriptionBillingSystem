using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionBillingSystem.Domain.Aggregates.InvoiceAggregate;

namespace SubscriptionBillingSystem.Infrastructure.Persistence.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SubscriptionId)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            // =========================
            // VALUE OBJECT: Money
            // =========================
            builder.OwnsOne(x => x.Amount, money =>
            {
                money.Property(m => m.Amount)
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasMaxLength(3)
                    .IsRequired();
            });
        }
    }
}