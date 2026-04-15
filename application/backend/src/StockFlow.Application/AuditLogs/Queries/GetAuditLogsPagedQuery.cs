using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.AuditLogs.Queries;

public record GetAuditLogsPagedQuery(AuditLogFilterParams FilterParams) : IRequest<PagedResult<AuditLogDto>>;
