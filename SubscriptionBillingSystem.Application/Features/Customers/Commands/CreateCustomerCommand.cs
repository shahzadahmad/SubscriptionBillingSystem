using MediatR;

namespace SubscriptionBillingSystem.Application.Features.Customers.Commands
{
    public class CreateCustomerCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}