using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.Orders.Queries;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IRepository<Domain.Entities.Order> _orderRepository;
    private readonly IMapper _mapper;

    public GetOrderByIdQueryHandler(IRepository<Domain.Entities.Order> orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);
        return order != null ? _mapper.Map<OrderDto>(order) : null;
    }
}
