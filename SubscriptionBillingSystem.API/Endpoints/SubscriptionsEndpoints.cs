using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionBillingSystem.Application.Common.Interfaces;
using SubscriptionBillingSystem.Application.Features.Subscriptions.Commands;

namespace SubscriptionBillingSystem.Api.Endpoints
{
    public static class SubscriptionsEndpoints
    {
        public static IEndpointRouteBuilder MapSubscriptionEndpoints(this IEndpointRouteBuilder app)
        {
            // =========================
            // CREATE SUBSCRIPTION
            // =========================
            app.MapPost("/subscriptions", async (
                CreateSubscriptionCommand command,
                IMediator mediator) =>
            {
                var result = await mediator.Send(command);
                return Results.Ok(result);
            });

            // =========================
            // ACTIVATE SUBSCRIPTION
            // =========================
            app.MapPost("/subscriptions/activate", async (
                ActivateSubscriptionCommand command,
                IMediator mediator) =>
            {
                await mediator.Send(command);
                return Results.Ok();
            });

            // =========================
            // CANCEL SUBSCRIPTION
            // =========================
            app.MapPost("/subscriptions/cancel", async (
                CancelSubscriptionCommand command,
                IMediator mediator) =>
            {
                await mediator.Send(command);
                return Results.Ok();
            });

            // =========================
            // 🔍 DEBUG ENDPOINT
            // =========================
            app.MapGet("/debug/subscriptions", async (
                IApplicationDbContext context) =>
            {
                var data = await context.Subscriptions.ToListAsync();
                return Results.Ok(data);
            });

            return app;
        }
    }
}