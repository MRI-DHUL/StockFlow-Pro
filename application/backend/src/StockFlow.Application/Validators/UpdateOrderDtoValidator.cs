using FluentValidation;
using StockFlow.Application.DTOs;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.Validators;

public class UpdateOrderDtoValidator : AbstractValidator<UpdateOrderDto>
{
    public UpdateOrderDtoValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid order status");
    }
}
