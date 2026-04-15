using MediatR;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using StockFlow.Application.DTOs;
using StockFlow.Application.PurchaseOrders.Commands;
using StockFlow.Application.PurchaseOrders.Queries;

namespace StockFlow.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public PurchaseOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all purchase orders
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PurchaseOrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PurchaseOrderDto>>> GetAll()
    {
        var purchaseOrders = await _mediator.Send(new GetAllPurchaseOrdersQuery());
        return Ok(purchaseOrders);
    }

    /// <summary>
    /// Get purchase order by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseOrderDto>> GetById(Guid id)
    {
        var purchaseOrder = await _mediator.Send(new GetPurchaseOrderByIdQuery(id));
        
        if (purchaseOrder == null)
            return NotFound(new { message = $"Purchase order with ID {id} not found." });

        return Ok(purchaseOrder);
    }

    /// <summary>
    /// Create a new purchase order
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PurchaseOrderDto>> Create([FromBody] CreatePurchaseOrderDto createPurchaseOrderDto)
    {
        var purchaseOrder = await _mediator.Send(new CreatePurchaseOrderCommand(createPurchaseOrderDto));
        return CreatedAtAction(nameof(GetById), new { id = purchaseOrder.Id }, purchaseOrder);
    }

    /// <summary>
    /// Update purchase order status
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseOrderDto>> Update(Guid id, [FromBody] UpdatePurchaseOrderDto updatePurchaseOrderDto)
    {
        var purchaseOrder = await _mediator.Send(new UpdatePurchaseOrderCommand(id, updatePurchaseOrderDto));
        
        if (purchaseOrder == null)
            return NotFound(new { message = $"Purchase order with ID {id} not found." });

        return Ok(purchaseOrder);
    }
}
