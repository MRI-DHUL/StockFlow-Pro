using FluentValidation;
using StockFlow.Application.DTOs;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.Validators;

public class UpdatePurchaseOrderDtoValidator : AbstractValidator<UpdatePurchaseOrderDto>
{
    public UpdatePurchaseOrderDtoValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid purchase order status");

        RuleFor(x => x.ActualDeliveryDate)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Actual delivery date cannot be in the future")
            .When(x => x.ActualDeliveryDate.HasValue);
    }
}
