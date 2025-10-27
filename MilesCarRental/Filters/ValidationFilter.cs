namespace MilesCarRental.Api.Filters;

using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MilesCarRental.Api.Models;

/// <summary>
/// Action filter that validates request DTOs using FluentValidation.
/// </summary>
public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Get all parameters that need validation
        foreach (var parameter in context.ActionDescriptor.Parameters)
        {
            if (context.ActionArguments.TryGetValue(parameter.Name, out var argument) && argument != null)
            {
                var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var validationContext = new ValidationContext<object>(argument);
                    var validationResult = await validator.ValidateAsync(validationContext);

                    if (!validationResult.IsValid)
                    {
                        var errorResponse = new ErrorResponse
                        {
                            Type = "https://api.milescarrental.com/errors/validation-error",
                            Title = "Validation Error",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "One or more validation errors occurred.",
                            Instance = context.HttpContext.Request.Path,
                            ErrorCode = "VALIDATION_ERROR",
                            TraceId = context.HttpContext.TraceIdentifier,
                            Extensions = new Dictionary<string, object>
                            {
                                ["errors"] = validationResult.Errors.Select(e => new
                                {
                                    field = e.PropertyName,
                                    message = e.ErrorMessage,
                                    attemptedValue = e.AttemptedValue,
                                    severity = e.Severity.ToString()
                                })
                            }
                        };

                        context.Result = new UnprocessableEntityObjectResult(errorResponse);
                        return;
                    }
                }
            }
        }

        await next();
    }
}
