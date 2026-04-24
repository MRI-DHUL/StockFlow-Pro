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

    /// <summary>
    /// Get audit logs with filtering (Admin only)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAll([FromQuery] AuditLogFilterParams filterParams)
    {
        var auditLogs = await _auditLogService.GetAllAsync(filterParams);
        return Ok(auditLogs);
    }

    /// <summary>
    /// Get audit logs for a specific entity (Admin only)
    /// </summary>
    [HttpGet("entity/{entityName}/{entityId}")]
    [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetByEntity(string entityName, Guid entityId)
    {
        var auditLogs = await _auditLogService.GetByEntityAsync(entityName, entityId);
        return Ok(auditLogs);
    }
}
