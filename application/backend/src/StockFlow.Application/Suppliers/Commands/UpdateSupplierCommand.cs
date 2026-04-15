using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Suppliers.Commands;

public record UpdateSupplierCommand(Guid Id, UpdateSupplierDto SupplierDto) : IRequest<SupplierDto?>;
