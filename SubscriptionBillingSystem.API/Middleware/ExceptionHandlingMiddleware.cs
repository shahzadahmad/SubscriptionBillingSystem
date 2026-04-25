using System.Net;
using System.Text.Json;
using SubscriptionBillingSystem.Application.Common.Exceptions;
using SubscriptionBillingSystem.Domain.Exceptions;
using SubscriptionBillingSystem.Api.Common;

namespace SubscriptionBillingSystem.Api.Middleware
{
    /// <summary>
    /// Global Exception Handling Middleware
    /// 
    /// Responsibilities:
    /// - Intercepts all unhandled exceptions in the HTTP pipeline
    /// - Logs the exception details
    /// - Converts exceptions into standardized JSON API responses
    /// - Prevents leaking sensitive/internal details to clients
    /// 
    /// This ensures:
    /// - Consistent error responses across the API
    /// - Better debugging via centralized logging
    /// - Cleaner controller/service code (no repetitive try-catch blocks)
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        /// <summary>
        /// Delegate to invoke the next middleware in the pipeline
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// Logger for capturing exception details
        /// </summary>
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Constructor for middleware
        /// </summary>
        /// <param name="next">Next middleware in the pipeline</param>
        /// <param name="logger">Logger instance</param>
        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Middleware execution method
        /// 
        /// Flow:
        /// 1. Pass request to next middleware
        /// 2. Catch any unhandled exception
        /// 3. Log the error
        /// 4. Return standardized error response
        /// </summary>
        /// <param name="context">HTTP context</param>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Continue request pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log full exception details (stack trace, message, etc.)
                _logger.LogError(ex, "Unhandled exception occurred");

                // Convert exception into API response
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles exception and writes a structured JSON response
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="exception">Captured exception</param>
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Set response content type
            context.Response.ContentType = "application/json";

            // Create structured error response based on exception type
            var response = CreateErrorResponse(exception);

            // Set appropriate HTTP status code
            context.Response.StatusCode = response.StatusCode;

            // Serialize response to JSON
            var json = JsonSerializer.Serialize(response);

            // Write response to client
            await context.Response.WriteAsync(json);
        }

        /// <summary>
        /// Maps different exception types to appropriate HTTP responses
        /// 
        /// Exception Handling Strategy:
        /// - NotFoundException → 404 Not Found
        /// - ValidationException → 400 Bad Request (with validation details)
        /// - DomainException → 400 Bad Request (business rule violation)
        /// - Unknown Exception → 500 Internal Server Error
        /// </summary>
        /// <param name="exception">Exception to map</param>
        /// <returns>Standardized error response</returns>
        private static ErrorResponse CreateErrorResponse(Exception exception)
        {
            return exception switch
            {
                // Resource not found (e.g., entity does not exist)
                NotFoundException nf =>
                    ErrorResponse.From(
                        HttpStatusCode.NotFound,
                        nf.Message),

                // FluentValidation exception (input validation errors)
                FluentValidation.ValidationException ex =>
                    ErrorResponse.From(
                        HttpStatusCode.BadRequest,
                        "Validation failed",
                        ex.Errors.Select(e => new
                        {
                            propertyName = e.PropertyName,
                            errorMessage = e.ErrorMessage
                        }).ToList()),

                // Domain/business rule violations
                DomainException de =>
                    ErrorResponse.From(
                        HttpStatusCode.BadRequest,
                        de.Message),

                // Fallback for unhandled exceptions
                _ =>
                    ErrorResponse.From(
                        HttpStatusCode.InternalServerError,
                        "An unexpected error occurred")
            };
        }
    }
}