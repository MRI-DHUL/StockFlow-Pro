using StockFlow.Domain.Entities;

namespace StockFlow.Application.Interfaces;

public interface IAuditLogRepository
{
    Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    IQueryable<AuditLog> Query();
}
