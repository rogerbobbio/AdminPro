using System.Text.Json;
using ValidationException = AdminPro.Application.Common.Exceptions.ValidationException;

namespace AdminPro.Api.Middleware;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        object body;

        if (exception is ValidationException validationException)
        {
            logger.LogWarning(exception, "Validation error");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            body = new
            {
                error = "ValidationError",
                message = exception.Message,
                details = validationException.Errors
                    .SelectMany(e => e.Value.Select(message => new { field = e.Key, error = message }))
            };
        }
        else
        {
            logger.LogError(exception, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            body = new
            {
                error = "InternalServerError",
                message = "An unexpected error occurred.",
                details = Array.Empty<object>()
            };
        }

        await context.Response.WriteAsync(JsonSerializer.Serialize(body));
    }
}
