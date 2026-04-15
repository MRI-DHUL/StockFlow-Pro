using StockFlow.Domain.Entities;

namespace StockFlow.Application.Interfaces;

public interface IInventoryRepository : IRepository<Domain.Entities.Inventory>
{
    Task<IEnumerable<Domain.Entities.Inventory>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Inventory>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<Domain.Entities.Inventory?> GetByProductAndWarehouseAsync(Guid productId, Guid warehouseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Inventory>> GetLowStockItemsAsync(CancellationToken cancellationToken = default);
}
