using MediatR;
using SubscriptionBillingSystem.Application.Common.Interfaces;

namespace SubscriptionBillingSystem.Application.Features.Subscriptions.Commands
{
    public class CreateSubscriptionCommand : ICommand<Guid>
    {
        public Guid CustomerId { get; set; }
        public decimal MonthlyPrice { get; set; }
        public string Currency { get; set; }
    }
}