using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogsController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Get audit logs with pagination, filtering, and sorting (Admin only)
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResult<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AuditLogDto>>> GetPaged([FromQuery] AuditLogFilterParams filterParams)
    {
        var auditLogs = await _auditLogService.GetPagedAsync(filterParams);
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
        var auditLog = await _auditLogService.GetByIdAsync(id);
        
        if (auditLog == null)
            return NotFound();

        return Ok(auditLog);
    }
}
