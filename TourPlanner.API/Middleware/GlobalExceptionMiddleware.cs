using System.Net;
using System.Text.Json;
using TourPlanner.BL.Exceptions;

namespace TourPlanner.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(BusinessException ex)
            {
                _logger.LogWarning(ex, "Business exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex, DetermineStatusCode(ex));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
            }
        }

        private static int DetermineStatusCode(Exception ex)
        {
            if(ex.Message.Contains("Unauthorised", StringComparison.OrdinalIgnoreCase)) return (int)HttpStatusCode.Forbidden;
            if(ex.Message.Contains("Not Found", StringComparison.OrdinalIgnoreCase)) return (int)HttpStatusCode.NotFound;
            if(ex.Message.Contains("Bad Request", StringComparison.OrdinalIgnoreCase)) return (int)HttpStatusCode.BadRequest;
            if(ex.Message.Contains("Conflict", StringComparison.OrdinalIgnoreCase)) return (int)HttpStatusCode.Conflict;

            if(ex.InnerException != null)
            {
                return DetermineStatusCode(ex.InnerException);
            }

            return (int)HttpStatusCode.BadRequest;
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statuscode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statuscode;

            var response = new
            {
                Error = statuscode == HttpStatusCode.InternalServerError ? "An unexpected error occurred." : exception.Message
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, int statuscode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statuscode;

            var response = new { Error = exception.Message };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}