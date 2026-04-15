using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.PurchaseOrders.Commands;

public record CreatePurchaseOrderCommand(CreatePurchaseOrderDto PurchaseOrderDto) : IRequest<PurchaseOrderDto>;
