using System.Net;
using System.Text.Json;
using FluentValidation;

namespace StockFlow.API.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;
        var path = context.Request.Path;
        var method = context.Request.Method;

        _logger.LogError(exception, 
            "Unhandled exception occurred. TraceId: {TraceId}, Path: {Path}, Method: {Method}", 
            traceId, path, method);

        context.Response.ContentType = "application/json";

        var statusCode = exception switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var response = exception switch
        {
            ValidationException validationException => new ErrorResponse
            {
                StatusCode = statusCode,
                Message = "Validation failed",
                TraceId = traceId,
                Errors = validationException.Errors.Select(e => new ValidationError
                {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                }).ToList()
            },
            KeyNotFoundException => new ErrorResponse
            {
                StatusCode = statusCode,
                Message = exception.Message,
                TraceId = traceId
            },
            UnauthorizedAccessException => new ErrorResponse
            {
                StatusCode = statusCode,
                Message = "Unauthorized access",
                TraceId = traceId
            },
            ArgumentException => new ErrorResponse
            {
                StatusCode = statusCode,
                Message = exception.Message,
                TraceId = traceId
            },
            _ => new ErrorResponse
            {
                StatusCode = statusCode,
                Message = "An internal server error occurred. Please contact support.",
                TraceId = traceId
            }
        };

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? TraceId { get; set; }
    public IEnumerable<ValidationError>? Errors { get; set; }
}

public class ValidationError
{
    public string Property { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
}
