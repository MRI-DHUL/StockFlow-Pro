using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StockFlow.Application.DTOs;
using StockFlow.Application.Extensions;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.Orders.Queries;

public class GetOrdersPagedQueryHandler : IRequestHandler<GetOrdersPagedQuery, PagedResult<OrderDto>>
{
    private readonly IRepository<Domain.Entities.Order> _orderRepository;
    private readonly IMapper _mapper;

    public GetOrdersPagedQueryHandler(IRepository<Domain.Entities.Order> orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<OrderDto>> Handle(GetOrdersPagedQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Domain.Entities.Order> query = _orderRepository.Query()
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.FilterParams.OrderNumber))
        {
            query = query.Where(o => o.OrderNumber.Contains(request.FilterParams.OrderNumber));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterParams.CustomerName))
        {
            var searchTerm = request.FilterParams.CustomerName.ToLower();
            query = query.Where(o => o.CustomerName != null && o.CustomerName.ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterParams.Status))
        {
            if (Enum.TryParse<Domain.Enums.OrderStatus>(request.FilterParams.Status, true, out var status))
            {
                query = query.Where(o => o.Status == status);
            }
        }

        if (request.FilterParams.CreatedFrom.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= request.FilterParams.CreatedFrom.Value);
        }

        if (request.FilterParams.CreatedTo.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= request.FilterParams.CreatedTo.Value);
        }

        if (request.FilterParams.MinTotalAmount.HasValue)
        {
            query = query.Where(o => o.TotalAmount >= request.FilterParams.MinTotalAmount.Value);
        }

        if (request.FilterParams.MaxTotalAmount.HasValue)
        {
            query = query.Where(o => o.TotalAmount <= request.FilterParams.MaxTotalAmount.Value);
        }

        // Apply sorting
        query = query.ApplySorting(
            request.FilterParams.SortBy ?? "CreatedAt",
            request.FilterParams.SortDescending);

        // Get paged result
        var pagedOrders = await query.ToPagedResultAsync(
            request.FilterParams.PageNumber,
            request.FilterParams.PageSize,
            cancellationToken);

        return new PagedResult<OrderDto>
        {
            Items = _mapper.Map<List<OrderDto>>(pagedOrders.Items),
            PageNumber = pagedOrders.PageNumber,
            PageSize = pagedOrders.PageSize,
            TotalCount = pagedOrders.TotalCount
        };
    }
}
