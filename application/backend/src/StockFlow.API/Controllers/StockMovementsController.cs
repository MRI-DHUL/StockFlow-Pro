using MediatR;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using StockFlow.Application.DTOs;
using StockFlow.Application.StockMovements.Commands;
using StockFlow.Application.StockMovements.Queries;

namespace StockFlow.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class StockMovementsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StockMovementsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all stock movements
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StockMovementDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetAll()
    {
        var movements = await _mediator.Send(new GetAllStockMovementsQuery());
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
        var movement = await _mediator.Send(new GetStockMovementByIdQuery(id));
        
        if (movement == null)
            return NotFound(new { message = $"Stock movement with ID {id} not found." });

        return Ok(movement);
    }

    /// <summary>
    /// Create a new stock movement (automatically updates inventory)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(StockMovementDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StockMovementDto>> Create([FromBody] CreateStockMovementDto createStockMovementDto)
    {
        var movement = await _mediator.Send(new CreateStockMovementCommand(createStockMovementDto));
        return CreatedAtAction(nameof(GetById), new { id = movement.Id }, movement);
    }
}
