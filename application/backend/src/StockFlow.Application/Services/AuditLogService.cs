using Mapster;
using Microsoft.EntityFrameworkCore;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditLogService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<PagedResult<AuditLogDto>> GetPagedAsync(AuditLogFilterParams filterParams, CancellationToken cancellationToken = default)
    {
        var query = _auditLogRepository.Query();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(filterParams.UserId))
        {
            query = query.Where(a => a.UserId == filterParams.UserId);
        }

        if (!string.IsNullOrWhiteSpace(filterParams.EntityName))
        {
            query = query.Where(a => a.EntityName.Contains(filterParams.EntityName));
        }

        if (!string.IsNullOrWhiteSpace(filterParams.Action))
        {
            query = query.Where(a => a.Action == filterParams.Action);
        }

        if (filterParams.StartDate.HasValue)
        {
            query = query.Where(a => a.Timestamp >= filterParams.StartDate.Value);
        }

        if (filterParams.EndDate.HasValue)
        {
            query = query.Where(a => a.Timestamp <= filterParams.EndDate.Value);
        }

        // Apply sorting
        query = filterParams.SortBy?.ToLower() switch
        {
            "timestamp" => filterParams.SortDescending
                ? query.OrderByDescending(a => a.Timestamp)
                : query.OrderBy(a => a.Timestamp),
            "entityname" => filterParams.SortDescending
                ? query.OrderByDescending(a => a.EntityName)
                : query.OrderBy(a => a.EntityName),
            "action" => filterParams.SortDescending
                ? query.OrderByDescending(a => a.Action)
                : query.OrderBy(a => a.Action),
            _ => query.OrderByDescending(a => a.Timestamp) // Default sort by timestamp descending
        };

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var auditLogs = await query
            .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
            .Take(filterParams.PageSize)
            .ToListAsync(cancellationToken);

        var auditLogDtos = auditLogs.Adapt<List<AuditLogDto>>();

        return new PagedResult<AuditLogDto>
        {
            Items = auditLogDtos,
            TotalCount = totalCount,
            PageNumber = filterParams.PageNumber,
            PageSize = filterParams.PageSize
        };
    }

    public async Task<AuditLogDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var auditLog = await _auditLogRepository.GetByIdAsync(id, cancellationToken);
        return auditLog?.Adapt<AuditLogDto>();
    }
}
