using SubscriptionBillingSystem.Api.Endpoints;
using SubscriptionBillingSystem.Api.Middleware;
using SubscriptionBillingSystem.Application;
using SubscriptionBillingSystem.Infrastructure;
using SubscriptionBillingSystem.Infrastructure.BackgroundJobs;

var builder = WebApplication.CreateBuilder(args);

#region ========================= SERVICE REGISTRATION =========================

/*
 * Swagger / OpenAPI
 * Used for API documentation and testing endpoints in development.
 */
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*
 * APPLICATION LAYER
 * ---------------------------------------------
 * Contains:
 *   - CQRS (Commands / Queries)
 *   - MediatR handlers
 *   - Validation (FluentValidation)
 *   - Business use-cases orchestration
 */
builder.Services.AddApplication();

/*
 * INFRASTRUCTURE LAYER
 * ---------------------------------------------
 * Contains:
 *   - EF Core DbContext
 *   - External services
 *   - System abstractions (DateTime, etc.)
 *   - Persistence logic
 */
builder.Services.AddInfrastructure(builder.Configuration);

#endregion

var app = builder.Build();

#region ========================= HTTP PIPELINE =========================

/*
 * Development-only tools
 * ---------------------------------------------
 * Swagger UI enabled only in development environment
 */
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/*
 * HTTPS enforcement
 * ---------------------------------------------
 * Redirects all HTTP requests to HTTPS
 */
app.UseHttpsRedirection();

/*
 * Global Exception Handling Middleware
 * ---------------------------------------------
 * Centralized error handling (to be improved later with custom middleware)
 */
app.UseMiddleware<ExceptionHandlingMiddleware>();

#endregion

#region ========================= API ENDPOINTS =========================

/*
 * Root endpoint - simple health check
 */
app.MapGet("/", () => Results.Ok("Subscription Billing System API Running 🚀"));

/*
 * Feature-based endpoint mapping (Clean Architecture style)
 */
app.MapCustomerEndpoints();
app.MapSubscriptionEndpoints();
app.MapInvoiceEndpoints();

#endregion

app.Run();