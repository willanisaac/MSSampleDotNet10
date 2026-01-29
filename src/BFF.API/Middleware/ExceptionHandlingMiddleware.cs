using System.Net;
using System.Text.Json;
using BFF.Application.Exceptions;
using BFF.Domain.Models.Common;

namespace BFF.API.Middleware;

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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var errorResult = new ErrorResult
        {
            Timestamp = DateTime.UtcNow
        };

        context.Response.ContentType = "application/json";

        switch (exception)
        {
            case ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResult.Code = "VALIDATION_ERROR";
                errorResult.Message = validationEx.Message;
                errorResult.Details = validationEx.Errors;
                break;

            case GraphQLClientException graphQLEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadGateway;
                errorResult.Code = "GRAPHQL_ERROR";
                errorResult.Message = graphQLEx.Message;
                break;

            case ServiceUnavailableException serviceEx:
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                errorResult.Code = "SERVICE_UNAVAILABLE";
                errorResult.Message = serviceEx.Message;
                break;

            case InvalidOperationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResult.Code = "INVALID_OPERATION";
                errorResult.Message = exception.Message;
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResult.Code = "INTERNAL_SERVER_ERROR";
                errorResult.Message = "An internal server error occurred";
                break;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(errorResult, options);
        await context.Response.WriteAsync(json);
    }
}
