using System.Text.Json;
using ContestParticipationSystem.Application.Common;
using ContestParticipationSystem.Application.Exceptions;

namespace ContestParticipationSystem.Api.Middleware;

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
        catch (AppException exception)
        {
            context.Response.StatusCode = exception.StatusCode;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.FailureResponse(exception.Message);
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception occurred.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.FailureResponse("An unexpected error occurred.");
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
