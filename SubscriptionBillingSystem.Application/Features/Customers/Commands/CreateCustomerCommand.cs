using SubscriptionBillingSystem.Application.Common.Interfaces;

namespace SubscriptionBillingSystem.Application.Features.Customers.Commands
{
    public class CreateCustomerCommand : ICommand<Guid>
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}