using MediatR;

namespace SubscriptionBillingSystem.Application.Features.Subscriptions.Commands
{
    public class CreateSubscriptionCommand : IRequest<Guid>
    {
        public Guid CustomerId { get; set; }
        public decimal MonthlyPrice { get; set; }
        public string Currency { get; set; }
    }
}