using FluentValidation;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Validators;

public class CreatePurchaseOrderDtoValidator : AbstractValidator<CreatePurchaseOrderDto>
{
    public CreatePurchaseOrderDtoValidator()
    {
        RuleFor(x => x.SupplierId)
            .NotEmpty().WithMessage("Supplier ID is required");

        RuleFor(x => x.ExpectedDeliveryDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Expected delivery date must be in the future");

        RuleFor(x => x.PurchaseOrderItems)
            .NotEmpty().WithMessage("Purchase order must contain at least one item");

        RuleForEach(x => x.PurchaseOrderItems)
            .SetValidator(new CreatePurchaseOrderItemDtoValidator());
    }
}

public class CreatePurchaseOrderItemDtoValidator : AbstractValidator<CreatePurchaseOrderItemDto>
{
    public CreatePurchaseOrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than 0");
    }
}
