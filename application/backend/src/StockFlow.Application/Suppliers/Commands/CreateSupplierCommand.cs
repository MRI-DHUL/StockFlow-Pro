using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Suppliers.Commands;

public record CreateSupplierCommand(CreateSupplierDto SupplierDto) : IRequest<SupplierDto>;
