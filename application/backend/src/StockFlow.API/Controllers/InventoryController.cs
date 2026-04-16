using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.API.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    /// <summary>
    /// Get inventory with pagination, filtering, and sorting
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResult<InventoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<InventoryDto>>> GetPaged([FromQuery] InventoryFilterParams filterParams)
    {
        var inventory = await _inventoryService.GetPagedAsync(filterParams);
        return Ok(inventory);
    }

    /// <summary>
    /// Get all inventory items
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InventoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<InventoryDto>>> GetAll()
    {
        var inventory = await _inventoryService.GetAllAsync();
        return Ok(inventory);
    }

    /// <summary>
    /// Get low stock items
    /// </summary>
    [HttpGet("low-stock")]
    [ProducesResponseType(typeof(IEnumerable<InventoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<InventoryDto>>> GetLowStock()
    {
        var lowStockItems = await _inventoryService.GetLowStockItemsAsync();
        return Ok(lowStockItems);
    }

    /// <summary>
    /// Create a new inventory record
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(InventoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InventoryDto>> Create([FromBody] CreateInventoryDto createInventoryDto)
    {
        var inventory = await _inventoryService.CreateAsync(createInventoryDto);
        return CreatedAtAction(nameof(GetAll), new { id = inventory.Id }, inventory);
    }

    /// <summary>
    /// Update inventory quantity and threshold
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(InventoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InventoryDto>> Update(Guid id, [FromBody] UpdateInventoryDto updateInventoryDto)
    {
        var inventory = await _inventoryService.UpdateAsync(id, updateInventoryDto);
        
        if (inventory == null)
            return NotFound(new { message = $"Inventory with ID {id} not found." });

        return Ok(inventory);
    }
}
