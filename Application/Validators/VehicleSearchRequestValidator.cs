namespace MilesCarRental.Application.Validators;

using FluentValidation;
using MilesCarRental.Application.DTOs;

public class VehicleSearchRequestValidator : AbstractValidator<VehicleSearchRequestDto>
{
    public VehicleSearchRequestValidator()
    {
        RuleFor(x => x.PickupLocation)
            .NotEmpty().WithMessage("Pickup location is required")
            .MaximumLength(50).WithMessage("Pickup location must not exceed 50 characters")
            .Matches("^[A-Z]+$").WithMessage("Pickup location must contain only uppercase letters");

        RuleFor(x => x.ReturnLocation)
            .NotEmpty().WithMessage("Return location is required")
            .MaximumLength(50).WithMessage("Return location must not exceed 50 characters")
            .Matches("^[A-Z]+$").WithMessage("Return location must contain only uppercase letters");

        RuleFor(x => x.ClassCode)
            .MaximumLength(20).When(x => !string.IsNullOrEmpty(x.ClassCode))
  .WithMessage("Class code must not exceed 20 characters");

        RuleFor(x => x.Page)
    .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");
    }
}
