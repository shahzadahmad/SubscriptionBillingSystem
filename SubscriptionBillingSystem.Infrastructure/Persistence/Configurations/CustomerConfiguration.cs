using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionBillingSystem.Domain.Aggregates.CustomerAggregate;

namespace SubscriptionBillingSystem.Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Value Object: Email
            builder.OwnsOne(x => x.Email, email =>
            {
                email.Property(e => e.Value)
                    .IsRequired()                    
                    .HasMaxLength(255);
            });            

            // One Customer → Many Subscriptions
            builder.HasMany(x => x.Subscriptions)
                .WithOne()
                .HasForeignKey("CustomerId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}