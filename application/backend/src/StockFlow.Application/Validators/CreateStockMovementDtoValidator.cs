using FluentValidation;
using StockFlow.Application.DTOs;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.Validators;

public class CreateStockMovementDtoValidator : AbstractValidator<CreateStockMovementDto>
{
    public CreateStockMovementDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid movement type");

        RuleFor(x => x.FromWarehouseId)
            .NotEmpty().WithMessage("From warehouse is required for Out and Transfer movements")
            .When(x => x.Type == MovementType.Out || x.Type == MovementType.Transfer);

        RuleFor(x => x.ToWarehouseId)
            .NotEmpty().WithMessage("To warehouse is required for In and Transfer movements")
            .When(x => x.Type == MovementType.In || x.Type == MovementType.Transfer);

        RuleFor(x => x.ToWarehouseId)
            .NotEqual(x => x.FromWarehouseId).WithMessage("From and To warehouses must be different")
            .When(x => x.Type == MovementType.Transfer && x.FromWarehouseId.HasValue && x.ToWarehouseId.HasValue);

        RuleFor(x => x.Reference)
            .MaximumLength(100).WithMessage("Reference cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Reference));

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
