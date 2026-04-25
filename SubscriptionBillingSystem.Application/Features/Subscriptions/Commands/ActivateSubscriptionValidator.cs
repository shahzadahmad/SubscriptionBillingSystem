using FluentValidation;

namespace SubscriptionBillingSystem.Application.Features.Subscriptions.Commands
{
    public class ActivateSubscriptionValidator : AbstractValidator<ActivateSubscriptionCommand>
    {
        public ActivateSubscriptionValidator()
        {
            RuleFor(x => x.SubscriptionId).NotEmpty();            
        }
    }
}