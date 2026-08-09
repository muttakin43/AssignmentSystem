using AssignmentSystem.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace AssignmentSystem.API.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception occurred");

                var (statusCode, message) = ex switch
                {
                    NotFoundException => (HttpStatusCode.NotFound, ex.Message),
                    ConflictException => (HttpStatusCode.Conflict, ex.Message),
                    ValidationException => (HttpStatusCode.BadRequest, ex.Message),
                    BusinessRuleException => (HttpStatusCode.BadRequest, ex.Message),
                    ForbiddenException => (HttpStatusCode.Forbidden, ex.Message),
                    _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
                };

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;

                var result = JsonSerializer.Serialize(new { message });
                await context.Response.WriteAsync(result);
            }
        }
    }
}
