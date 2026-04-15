using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Products.Commands;

public record UpdateProductCommand(Guid Id, UpdateProductDto ProductDto) : IRequest<ProductDto?>;
