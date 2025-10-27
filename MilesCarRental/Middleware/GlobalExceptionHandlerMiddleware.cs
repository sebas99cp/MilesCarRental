namespace MilesCarRental.Api.Middleware;

using System.Net;
using System.Text.Json;
using MilesCarRental.Api.Models;
using MilesCarRental.Domain.Exceptions;

/// <summary>
/// Global exception handling middleware that catches all unhandled exceptions
/// and converts them to standardized error responses.
/// </summary>
public sealed class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
  RequestDelegate next,
     ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // ✅ Skip middleware completely for Swagger paths
        if (context.Request.Path.StartsWithSegments("/swagger") ||
     context.Request.Path.Value == "/" ||
context.Request.Path.Value == "/index.html")
        {
            await _next(context);
            return;
        }

        try
        {
            await _next(context);

            // ✅ Handle 404s for API endpoints only
            if (context.Response.StatusCode == 404 &&
         !context.Response.HasStarted &&
      context.Request.Path.StartsWithSegments("/api"))
            {
                var errorResponse = new ErrorResponse
                {
                    Type = "https://api.milescarrental.com/errors/not-found",
                    Title = "Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"The requested resource '{context.Request.Path}' was not found.",
                    Instance = context.Request.Path,
                    ErrorCode = "NOT_FOUND",
                    TraceId = context.TraceIdentifier
                };

                context.Response.ContentType = "application/problem+json";
                var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = _environment.IsDevelopment()
                });
                await context.Response.WriteAsync(json);
            }
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Log the exception
        LogException(exception, context);

        // Create error response
        var errorResponse = CreateErrorResponse(exception, context);

        // Set response
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = errorResponse.Status;

        // Serialize and write response
        var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        });

        await context.Response.WriteAsync(json);
    }

    private ErrorResponse CreateErrorResponse(Exception exception, HttpContext context)
    {
        return exception switch
        {
            // Domain exceptions
            DomainException domainException => new ErrorResponse
            {
                Type = $"https://api.milescarrental.com/errors/{domainException.ErrorCode.ToLowerInvariant().Replace('_', '-')}",
                Title = GetTitle(domainException),
                Status = domainException.StatusCode,
                Detail = domainException.Message,
                Instance = context.Request.Path,
                ErrorCode = domainException.ErrorCode,
                TraceId = context.TraceIdentifier,
                Extensions = GetExtensions(domainException)
            },

            // Validation exceptions
            ArgumentException argumentException => new ErrorResponse
            {
                Type = "https://api.milescarrental.com/errors/validation-error",
                Title = "Validation Error",
                Status = StatusCodes.Status400BadRequest,
                Detail = argumentException.Message,
                Instance = context.Request.Path,
                ErrorCode = "VALIDATION_ERROR",
                TraceId = context.TraceIdentifier
            },

            // Operation cancelled (client disconnected)
            OperationCanceledException => new ErrorResponse
            {
                Type = "https://api.milescarrental.com/errors/request-cancelled",
                Title = "Request Cancelled",
                Status = StatusCodes.Status499ClientClosedRequest,
                Detail = "The request was cancelled by the client.",
                Instance = context.Request.Path,
                ErrorCode = "REQUEST_CANCELLED",
                TraceId = context.TraceIdentifier
            },

            // Generic server error
            _ => new ErrorResponse
            {
                Type = "https://api.milescarrental.com/errors/internal-server-error",
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = _environment.IsDevelopment()
              ? exception.Message
  : "An unexpected error occurred. Please try again later.",
                Instance = context.Request.Path,
                ErrorCode = "INTERNAL_ERROR",
                TraceId = context.TraceIdentifier,
                Extensions = _environment.IsDevelopment()
       ? new Dictionary<string, object>
       {
           ["stackTrace"] = exception.StackTrace ?? "No stack trace available",
           ["exceptionType"] = exception.GetType().Name
       }
    : null
            }
        };
    }

    private void LogException(Exception exception, HttpContext context)
    {
        var logLevel = exception switch
        {
            DomainException => LogLevel.Warning,
            ArgumentException => LogLevel.Warning,
            OperationCanceledException => LogLevel.Information,
            _ => LogLevel.Error
        };

        _logger.Log(
            logLevel,
            exception,
 "Exception occurred: {ExceptionType} | Path: {Path} | Method: {Method} | TraceId: {TraceId}",
   exception.GetType().Name,
   context.Request.Path,
 context.Request.Method,
         context.TraceIdentifier);
    }

    private static string GetTitle(DomainException exception)
    {
        return exception switch
        {
            LocationNotFoundException => "Location Not Found",
            DepartmentMismatchException => "Department Mismatch",
            _ => "Domain Error"
        };
    }

    private static Dictionary<string, object>? GetExtensions(DomainException exception)
    {
        return exception switch
        {
            LocationNotFoundException ex => new Dictionary<string, object>
            {
                ["municipioId"] = ex.MunicipioId
            },
            DepartmentMismatchException ex => new Dictionary<string, object>
            {
                ["providedDepartment"] = ex.ProvidedDepartment,
                ["actualDepartment"] = ex.ActualDepartment,
                ["municipioId"] = ex.MunicipioId
            },
            _ => null
        };
    }
}

/// <summary>
/// Extension method to register the global exception handler middleware.
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
