using FluentValidation;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Validators;

public class UpdateInventoryDtoValidator : AbstractValidator<UpdateInventoryDto>
{
    public UpdateInventoryDtoValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative");

        RuleFor(x => x.Threshold)
            .GreaterThanOrEqualTo(0).WithMessage("Threshold cannot be negative");
    }
}
