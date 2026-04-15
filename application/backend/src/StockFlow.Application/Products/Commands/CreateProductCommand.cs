using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Products.Commands;

public record CreateProductCommand(CreateProductDto ProductDto) : IRequest<ProductDto>;
