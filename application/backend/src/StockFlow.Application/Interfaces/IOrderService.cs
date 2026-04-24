using StockFlow.Application.DTOs;

namespace StockFlow.Application.Interfaces;

public interface IOrderService
{
    Task<PagedResult<OrderDto>> GetPagedAsync(OrderFilterParams filterParams, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OrderDto> CreateAsync(CreateOrderDto createOrderDto, CancellationToken cancellationToken = default);
    Task<OrderDto?> UpdateAsync(Guid id, UpdateOrderDto updateOrderDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
