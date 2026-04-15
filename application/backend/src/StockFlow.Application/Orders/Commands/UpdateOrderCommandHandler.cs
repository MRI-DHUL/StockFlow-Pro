using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.Orders.Commands;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, OrderDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderDto?> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.Id, cancellationToken);
        
        if (order == null)
            return null;

        order.Status = request.OrderDto.Status;

        if (request.OrderDto.Status == OrderStatus.Delivered || request.OrderDto.Status == OrderStatus.Cancelled)
        {
            order.CompletedAt = DateTime.UtcNow;
        }

        await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = await _unitOfWork.Orders.GetByIdAsync(order.Id, cancellationToken);
        return result != null ? _mapper.Map<OrderDto>(result) : null;
    }
}
