using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Suppliers.Queries;

public record GetAllSuppliersQuery : IRequest<IEnumerable<SupplierDto>>;
