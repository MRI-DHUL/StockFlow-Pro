using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.AuditLogs.Queries;

public record GetAuditLogByIdQuery(Guid Id) : IRequest<AuditLogDto?>;
