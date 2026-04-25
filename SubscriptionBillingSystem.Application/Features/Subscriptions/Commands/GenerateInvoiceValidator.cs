using FluentValidation;

namespace SubscriptionBillingSystem.Application.Features.Subscriptions.Commands
{
    public class GenerateInvoiceValidator : AbstractValidator<GenerateInvoiceCommand>
    {
        public GenerateInvoiceValidator()
        {
            RuleFor(x => x.SubscriptionId)
                .NotEmpty()
                .WithMessage("SubscriptionId is required.");

            // Optional but useful safeguard
            RuleFor(x => x)
                .NotNull()
                .WithMessage("Request cannot be null.");
        }
    }
}