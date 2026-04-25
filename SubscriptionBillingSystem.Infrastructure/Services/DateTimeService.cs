using SubscriptionBillingSystem.Application.Common.Interfaces;

namespace SubscriptionBillingSystem.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}