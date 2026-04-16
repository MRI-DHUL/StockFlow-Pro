using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IPurchaseOrderService _purchaseOrderService;

    public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService)
    {
        _purchaseOrderService = purchaseOrderService;
    }

    /// <summary>
    /// Get all purchase orders
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PurchaseOrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PurchaseOrderDto>>> GetAll()
    {
        var purchaseOrders = await _purchaseOrderService.GetAllAsync();
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
        var purchaseOrder = await _purchaseOrderService.GetByIdAsync(id);
        
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
        var purchaseOrder = await _purchaseOrderService.CreateAsync(createPurchaseOrderDto);
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
        var purchaseOrder = await _purchaseOrderService.UpdateAsync(id, updatePurchaseOrderDto);
        
        if (purchaseOrder == null)
            return NotFound(new { message = $"Purchase order with ID {id} not found." });

        return Ok(purchaseOrder);
    }
}
