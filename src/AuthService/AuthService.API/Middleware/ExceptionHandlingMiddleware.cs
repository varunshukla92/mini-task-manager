using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace AuthService.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                await HandleExceptionAsync(context, ex);
            }
        }

        private static string GetTitle(HttpStatusCode statusCode) =>
        statusCode switch
        {
 
            HttpStatusCode.NotFound => "Resource not found",
            HttpStatusCode.Forbidden => "Forbidden",
            HttpStatusCode.Unauthorized => "Unauthorized",
            HttpStatusCode.BadRequest => "Bad request",
            _ => "Internal server error"
        };

        private static Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
        {
            var statusCode = exception switch
            {
                InvalidOperationException => HttpStatusCode.Conflict,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };

            var problemDetails = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = GetTitle(statusCode),
                Detail = exception.Message
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problemDetails.Status.Value;

            return context.Response.WriteAsync(
                JsonSerializer.Serialize(problemDetails));
        }



    }
}
