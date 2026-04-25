using MediatR;
using SubscriptionBillingSystem.Application.Features.Invoices.Models;

namespace SubscriptionBillingSystem.Application.Features.Invoices.Queries
{
    public class GetInvoicesQuery : IRequest<List<GetInvoicesDto>>
    {
        public Guid? SubscriptionId { get; }

        public GetInvoicesQuery(Guid? subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
    }
}