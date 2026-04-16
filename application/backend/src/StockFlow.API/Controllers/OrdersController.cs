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
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Get orders with pagination, filtering, and sorting
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResult<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<OrderDto>>> GetPaged([FromQuery] OrderFilterParams filterParams)
    {
        var orders = await _orderService.GetPagedAsync(filterParams);
        return Ok(orders);
    }

    /// <summary>
    /// Get all orders
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
    {
        var orders = await _orderService.GetAllAsync();
        return Ok(orders);
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetById(Guid id)
    {
        var order = await _orderService.GetByIdAsync(id);
        
        if (order == null)
            return NotFound(new { message = $"Order with ID {id} not found." });

        return Ok(order);
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto createOrderDto)
    {
        var order = await _orderService.CreateAsync(createOrderDto);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>
    /// Update order status
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> Update(Guid id, [FromBody] UpdateOrderDto updateOrderDto)
    {
        var order = await _orderService.UpdateAsync(id, updateOrderDto);
        
        if (order == null)
            return NotFound(new { message = $"Order with ID {id} not found." });

        return Ok(order);
    }
}
