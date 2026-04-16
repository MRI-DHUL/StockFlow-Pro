using StockFlow.Application.DTOs;

namespace StockFlow.Application.Interfaces;

public interface IAuditLogService
{
    Task<PagedResult<AuditLogDto>> GetPagedAsync(AuditLogFilterParams filterParams, CancellationToken cancellationToken = default);
    Task<AuditLogDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
