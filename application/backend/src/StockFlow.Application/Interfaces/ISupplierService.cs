using StockFlow.Application.DTOs;

namespace StockFlow.Application.Interfaces;

public interface ISupplierService
{
    Task<IEnumerable<SupplierDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SupplierDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SupplierDto> CreateAsync(CreateSupplierDto createSupplierDto, CancellationToken cancellationToken = default);
    Task<SupplierDto?> UpdateAsync(Guid id, UpdateSupplierDto updateSupplierDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
