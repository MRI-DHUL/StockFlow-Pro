using MediatR;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using StockFlow.Application.DTOs;
using StockFlow.Application.Warehouse.Commands;
using StockFlow.Application.Warehouse.Queries;

namespace StockFlow.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class WarehousesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WarehousesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all warehouses
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WarehouseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetAll()
    {
        var warehouses = await _mediator.Send(new GetAllWarehousesQuery());
        return Ok(warehouses);
    }

    /// <summary>
    /// Get warehouse by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WarehouseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WarehouseDto>> GetById(Guid id)
    {
        var warehouse = await _mediator.Send(new GetWarehouseByIdQuery(id));
        
        if (warehouse == null)
            return NotFound(new { message = $"Warehouse with ID {id} not found." });

        return Ok(warehouse);
    }

    /// <summary>
    /// Create a new warehouse
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WarehouseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WarehouseDto>> Create([FromBody] CreateWarehouseDto createWarehouseDto)
    {
        var warehouse = await _mediator.Send(new CreateWarehouseCommand(createWarehouseDto));
        return CreatedAtAction(nameof(GetById), new { id = warehouse.Id }, warehouse);
    }

    /// <summary>
    /// Update warehouse
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(WarehouseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WarehouseDto>> Update(Guid id, [FromBody] UpdateWarehouseDto updateWarehouseDto)
    {
        var warehouse = await _mediator.Send(new UpdateWarehouseCommand(id, updateWarehouseDto));
        
        if (warehouse == null)
            return NotFound(new { message = $"Warehouse with ID {id} not found." });

        return Ok(warehouse);
    }

    /// <summary>
    /// Delete warehouse
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteWarehouseCommand(id));
        
        if (!result)
            return NotFound(new { message = $"Warehouse with ID {id} not found." });

        return NoContent();
    }
}
