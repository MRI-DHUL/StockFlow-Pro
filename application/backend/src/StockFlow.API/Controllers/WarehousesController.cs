using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehousesController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    /// <summary>
    /// Get all warehouses
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WarehouseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetAll()
    {
        var warehouses = await _warehouseService.GetAllAsync();
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
        var warehouse = await _warehouseService.GetByIdAsync(id);
        
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
        var warehouse = await _warehouseService.CreateAsync(createWarehouseDto);
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
        var warehouse = await _warehouseService.UpdateAsync(id, updateWarehouseDto);
        
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
        var result = await _warehouseService.DeleteAsync(id);
        
        if (!result)
            return NotFound(new { message = $"Warehouse with ID {id} not found." });

        return NoContent();
    }
}
