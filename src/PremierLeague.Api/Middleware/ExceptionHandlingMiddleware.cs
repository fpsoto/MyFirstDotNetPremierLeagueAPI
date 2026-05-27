using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PremierLeague.Application.Common.Exceptions;
using PremierLeague.Domain.Exceptions;

namespace PremierLeague.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, problem) = exception switch
        {
            ValidationException ve => (HttpStatusCode.UnprocessableEntity, new ProblemDetails
            {
                Title = "Validation Error",
                Status = (int)HttpStatusCode.UnprocessableEntity,
                Detail = "One or more validation errors occurred.",
                Extensions = { ["errors"] = ve.Errors }
            }),
            NotFoundException nfe => (HttpStatusCode.NotFound, new ProblemDetails
            {
                Title = "Resource Not Found",
                Status = (int)HttpStatusCode.NotFound,
                Detail = nfe.Message
            }),
            DomainException de => (HttpStatusCode.BadRequest, new ProblemDetails
            {
                Title = "Domain Rule Violation",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = de.Message
            }),
            _ => (HttpStatusCode.InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = "An unexpected error occurred."
            })
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            logger.LogError(exception, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
        else
            logger.LogWarning(exception, "Handled exception on {Method} {Path}: {Message}", context.Request.Method, context.Request.Path, exception.Message);

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;
        problem.Instance = context.Request.Path;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
    }
}
