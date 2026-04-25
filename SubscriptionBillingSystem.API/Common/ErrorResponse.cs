using System.Net;

namespace SubscriptionBillingSystem.Api.Common
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Errors { get; set; }

        public static ErrorResponse From(HttpStatusCode code, string message, object? errors = null)
        {
            return new ErrorResponse
            {
                StatusCode = (int)code,
                Message = message,
                Errors = errors
            };
        }
    }
}