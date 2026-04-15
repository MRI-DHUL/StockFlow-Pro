using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using StockFlow.Application.DTOs;
using StockFlow.Application.AuditLogs.Queries;

namespace StockFlow.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get audit logs with pagination, filtering, and sorting (Admin only)
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResult<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AuditLogDto>>> GetPaged([FromQuery] AuditLogFilterParams filterParams)
    {
        var auditLogs = await _mediator.Send(new GetAuditLogsPagedQuery(filterParams));
        return Ok(auditLogs);
    }

    /// <summary>
    /// Get audit log by ID (Admin only)
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AuditLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuditLogDto>> GetById(Guid id)
    {
        var auditLog = await _mediator.Send(new GetAuditLogByIdQuery(id));
        
        if (auditLog == null)
            return NotFound();

        return Ok(auditLog);
    }
}
