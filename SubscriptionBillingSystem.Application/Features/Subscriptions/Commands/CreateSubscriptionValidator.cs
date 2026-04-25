using FluentValidation;

namespace SubscriptionBillingSystem.Application.Features.Subscriptions.Commands
{
    public class CreateSubscriptionValidator : AbstractValidator<CreateSubscriptionCommand>
    {
        public CreateSubscriptionValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.MonthlyPrice)
                .GreaterThan(0);

            RuleFor(x => x.Currency)
                .NotEmpty();
        }
    }
}