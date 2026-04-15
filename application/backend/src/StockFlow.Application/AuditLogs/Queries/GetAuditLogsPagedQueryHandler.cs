using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;
using StockFlow.Domain.Entities;

namespace StockFlow.Application.AuditLogs.Queries;

public class GetAuditLogsPagedQueryHandler : IRequestHandler<GetAuditLogsPagedQuery, PagedResult<AuditLogDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetAuditLogsPagedQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<PagedResult<AuditLogDto>> Handle(GetAuditLogsPagedQuery request, CancellationToken cancellationToken)
    {
        var query = _auditLogRepository.Query();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.FilterParams.UserId))
        {
            query = query.Where(a => a.UserId == request.FilterParams.UserId);
        }

        if (!string.IsNullOrWhiteSpace(request.FilterParams.EntityName))
        {
            query = query.Where(a => a.EntityName.Contains(request.FilterParams.EntityName));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterParams.Action))
        {
            query = query.Where(a => a.Action == request.FilterParams.Action);
        }

        if (request.FilterParams.StartDate.HasValue)
        {
            query = query.Where(a => a.Timestamp >= request.FilterParams.StartDate.Value);
        }

        if (request.FilterParams.EndDate.HasValue)
        {
            query = query.Where(a => a.Timestamp <= request.FilterParams.EndDate.Value);
        }

        // Apply sorting
        query = request.FilterParams.SortBy?.ToLower() switch
        {
            "timestamp" => request.FilterParams.SortDescending
                ? query.OrderByDescending(a => a.Timestamp)
                : query.OrderBy(a => a.Timestamp),
            "entityname" => request.FilterParams.SortDescending
                ? query.OrderByDescending(a => a.EntityName)
                : query.OrderBy(a => a.EntityName),
            "action" => request.FilterParams.SortDescending
                ? query.OrderByDescending(a => a.Action)
                : query.OrderBy(a => a.Action),
            _ => query.OrderByDescending(a => a.Timestamp) // Default sort by timestamp descending
        };

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var auditLogs = await query
            .Skip((request.FilterParams.PageNumber - 1) * request.FilterParams.PageSize)
            .Take(request.FilterParams.PageSize)
            .ToListAsync(cancellationToken);

        var auditLogDtos = auditLogs.Adapt<List<AuditLogDto>>();

        return new PagedResult<AuditLogDto>
        {
            Items = auditLogDtos,
            TotalCount = totalCount,
            PageNumber = request.FilterParams.PageNumber,
            PageSize = request.FilterParams.PageSize
        };
    }
}
