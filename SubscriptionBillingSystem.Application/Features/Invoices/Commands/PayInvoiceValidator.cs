using FluentValidation;

namespace SubscriptionBillingSystem.Application.Features.Invoices.Commands
{
    public class PayInvoiceValidator : AbstractValidator<PayInvoiceCommand>
    {
        public PayInvoiceValidator()
        {
            RuleFor(x => x.InvoiceId)
                .NotEmpty()
                .WithMessage("InvoiceId is required.");
        }
    }
}
