using MediatR;
using SubscriptionBillingSystem.Application.Features.Invoices.Commands;
using SubscriptionBillingSystem.Application.Features.Invoices.Queries;

namespace SubscriptionBillingSystem.Api.Endpoints
{
    public static class InvoicesEndpoints
    {
        public static IEndpointRouteBuilder MapInvoiceEndpoints(this IEndpointRouteBuilder app)
        {
            // =========================
            // PAY INVOICE
            // =========================
            app.MapPost("/invoices/pay", async (
                PayInvoiceCommand command,
                IMediator mediator) =>
            {
                await mediator.Send(command);
                return Results.Ok();
            });

            // =========================
            // GET INVOICES
            // =========================
            app.MapGet("/invoices/{subscriptionId:guid}", async (
                Guid subscriptionId,
                IMediator mediator) =>
            {
                var result = await mediator.Send(new GetInvoicesQuery(subscriptionId));
                return Results.Ok(result);
            });

            return app;
        }
    }
}