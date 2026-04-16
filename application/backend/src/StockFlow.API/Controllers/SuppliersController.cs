using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    /// <summary>
    /// Get all suppliers
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SupplierDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAll()
    {
        var suppliers = await _supplierService.GetAllAsync();
        return Ok(suppliers);
    }

    /// <summary>
    /// Get supplier by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SupplierDto>> GetById(Guid id)
    {
        var supplier = await _supplierService.GetByIdAsync(id);
        
        if (supplier == null)
            return NotFound(new { message = $"Supplier with ID {id} not found." });

        return Ok(supplier);
    }

    /// <summary>
    /// Create a new supplier
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SupplierDto>> Create([FromBody] CreateSupplierDto createSupplierDto)
    {
        var supplier = await _supplierService.CreateAsync(createSupplierDto);
        return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
    }

    /// <summary>
    /// Update supplier
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SupplierDto>> Update(Guid id, [FromBody] UpdateSupplierDto updateSupplierDto)
    {
        var supplier = await _supplierService.UpdateAsync(id, updateSupplierDto);
        
        if (supplier == null)
            return NotFound(new { message = $"Supplier with ID {id} not found." });

        return Ok(supplier);
    }

    /// <summary>
    /// Delete supplier
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _supplierService.DeleteAsync(id);
        
        if (!result)
            return NotFound(new { message = $"Supplier with ID {id} not found." });

        return NoContent();
    }
}
