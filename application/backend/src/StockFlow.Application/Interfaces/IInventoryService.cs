using StockFlow.Application.DTOs;

namespace StockFlow.Application.Interfaces;

public interface IInventoryService
{
    Task<PagedResult<InventoryDto>> GetPagedAsync(InventoryFilterParams filterParams, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<InventoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryDto>> GetLowStockItemsAsync(CancellationToken cancellationToken = default);
    Task<InventoryDto> CreateAsync(CreateInventoryDto createInventoryDto, CancellationToken cancellationToken = default);
    Task<InventoryDto?> UpdateAsync(Guid id, UpdateInventoryDto updateInventoryDto, CancellationToken cancellationToken = default);
}
