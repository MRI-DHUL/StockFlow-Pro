using StockFlow.Application.DTOs;

namespace StockFlow.Application.Interfaces;

public interface IWarehouseService
{
    Task<IEnumerable<WarehouseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WarehouseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WarehouseDto> CreateAsync(CreateWarehouseDto createWarehouseDto, CancellationToken cancellationToken = default);
    Task<WarehouseDto?> UpdateAsync(Guid id, UpdateWarehouseDto updateWarehouseDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
