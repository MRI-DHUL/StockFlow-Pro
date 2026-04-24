using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class StockMovementsController : ControllerBase
{
    private readonly IStockMovementService _stockMovementService;

    public StockMovementsController(IStockMovementService stockMovementService)
    {
        _stockMovementService = stockMovementService;
    }

    /// <summary>
    /// Get all stock movements
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StockMovementDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetAll()
    {
        var movements = await _stockMovementService.GetAllAsync();
        return Ok(movements);
    }

    /// <summary>
    /// Get stock movement by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StockMovementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StockMovementDto>> GetById(Guid id)
    {
        var movement = await _stockMovementService.GetByIdAsync(id);
        
        if (movement == null)
            return NotFound(new { message = $"Stock movement with ID {id} not found." });

        return Ok(movement);
    }

    /// <summary>
    /// Get stock movements by product ID
    /// </summary>
    [HttpGet("product/{productId}")]
    [ProducesResponseType(typeof(IEnumerable<StockMovementDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetByProductId(Guid productId)
    {
        var movements = await _stockMovementService.GetByProductIdAsync(productId);
        return Ok(movements);
    }

    /// <summary>
    /// Create a new stock movement (automatically updates inventory)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(StockMovementDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StockMovementDto>> Create([FromBody] CreateStockMovementDto createStockMovementDto)
    {
        var movement = await _stockMovementService.CreateAsync(createStockMovementDto);
        return CreatedAtAction(nameof(GetById), new { id = movement.Id }, movement);
    }
}
