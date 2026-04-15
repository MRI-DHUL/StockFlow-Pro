using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Suppliers.Queries;

public record GetSupplierByIdQuery(Guid Id) : IRequest<SupplierDto?>;
