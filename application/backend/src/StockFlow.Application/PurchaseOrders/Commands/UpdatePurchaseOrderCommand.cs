using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.PurchaseOrders.Commands;

public record UpdatePurchaseOrderCommand(Guid Id, UpdatePurchaseOrderDto PurchaseOrderDto) : IRequest<PurchaseOrderDto?>;
