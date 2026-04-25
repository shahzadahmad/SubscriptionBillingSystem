using MediatR;
using SubscriptionBillingSystem.Application.Features.Customers.Commands;

namespace SubscriptionBillingSystem.Api.Endpoints
{
    public static class CustomersEndpoints
    {
        public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/customers", async (CreateCustomerCommand command, IMediator mediator) =>
            {
                var result = await mediator.Send(command);
                return Results.Ok(result);
            });

            return app;
        }
    }
}