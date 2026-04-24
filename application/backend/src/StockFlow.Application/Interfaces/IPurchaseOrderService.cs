using StockFlow.Application.DTOs;

namespace StockFlow.Application.Interfaces;

public interface IPurchaseOrderService
{
    Task<IEnumerable<PurchaseOrderDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PurchaseOrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PurchaseOrderDto> CreateAsync(CreatePurchaseOrderDto createPurchaseOrderDto, CancellationToken cancellationToken = default);
    Task<PurchaseOrderDto?> UpdateAsync(Guid id, UpdatePurchaseOrderDto updatePurchaseOrderDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
