using StockFlow.Application.DTOs;

namespace StockFlow.Application.Interfaces;

public interface IStockMovementService
{
    Task<IEnumerable<StockMovementDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StockMovementDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StockMovementDto> CreateAsync(CreateStockMovementDto createStockMovementDto, CancellationToken cancellationToken = default);
}
