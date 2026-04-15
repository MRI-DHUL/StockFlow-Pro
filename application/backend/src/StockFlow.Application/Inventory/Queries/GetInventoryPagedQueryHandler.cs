using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StockFlow.Application.DTOs;
using StockFlow.Application.Extensions;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.Inventory.Queries;

public class GetInventoryPagedQueryHandler : IRequestHandler<GetInventoryPagedQuery, PagedResult<InventoryDto>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IMapper _mapper;

    public GetInventoryPagedQueryHandler(IInventoryRepository inventoryRepository, IMapper mapper)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<InventoryDto>> Handle(GetInventoryPagedQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Domain.Entities.Inventory> query = _inventoryRepository.Query()
            .Include(i => i.Product)
            .Include(i => i.Warehouse);

        // Apply filters
        if (request.FilterParams.ProductId.HasValue)
        {
            query = query.Where(i => i.ProductId == request.FilterParams.ProductId.Value);
        }

        if (request.FilterParams.WarehouseId.HasValue)
        {
            query = query.Where(i => i.WarehouseId == request.FilterParams.WarehouseId.Value);
        }

        if (request.FilterParams.MinQuantity.HasValue)
        {
            query = query.Where(i => i.Quantity >= request.FilterParams.MinQuantity.Value);
        }

        if (request.FilterParams.MaxQuantity.HasValue)
        {
            query = query.Where(i => i.Quantity <= request.FilterParams.MaxQuantity.Value);
        }

        // Apply sorting
        query = query.ApplySorting(
            request.FilterParams.SortBy ?? "Quantity",
            request.FilterParams.SortDescending);

        // Get paged result
        var pagedInventory = await query.ToPagedResultAsync(
            request.FilterParams.PageNumber,
            request.FilterParams.PageSize,
            cancellationToken);

        return new PagedResult<InventoryDto>
        {
            Items = _mapper.Map<List<InventoryDto>>(pagedInventory.Items),
            PageNumber = pagedInventory.PageNumber,
            PageSize = pagedInventory.PageSize,
            TotalCount = pagedInventory.TotalCount
        };
    }
}
