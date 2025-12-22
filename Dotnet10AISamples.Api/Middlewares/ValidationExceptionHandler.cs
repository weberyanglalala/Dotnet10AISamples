using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;

namespace Dotnet10AISamples.Api.Middlewares;

public sealed class ValidationExceptionHandler(
    ILogger<ValidationExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Exception type: {ExceptionType}, Message: {Message}", exception.GetType().Name, exception.Message);

        if (exception is not ValidationException validationException)
        {
            logger.LogInformation("Not a ValidationException, skipping");
            return false;
        }

        logger.LogInformation("Handling ValidationException with {ErrorCount} errors", validationException.Errors.Count());

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        httpContext.Response.ContentType = "application/problem+json";

        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key.ToLowerInvariant(),
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            title = "One or more validation errors occurred.",
            status = 400,
            errors = errors
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        logger.LogInformation("Validation error response written");
        return true;
    }
}