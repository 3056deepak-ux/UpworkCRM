using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.Inventory;

namespace UpworkERP.API.Controllers;

/// <summary>
/// Controller for Warehouse management (Inventory module)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WarehousesController : ControllerBase
{
    private readonly IService<Warehouse> _warehouseService;
    private readonly IActivityLogService _activityLogService;

    public WarehousesController(IService<Warehouse> warehouseService, IActivityLogService activityLogService)
    {
        _warehouseService = warehouseService;
        _activityLogService = activityLogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Warehouse>>> GetAll()
    {
        var warehouses = await _warehouseService.GetAllAsync();
        return Ok(warehouses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Warehouse>> GetById(int id)
    {
        var warehouse = await _warehouseService.GetByIdAsync(id);
        if (warehouse == null)
            return NotFound();
        return Ok(warehouse);
    }

    [HttpPost]
    public async Task<ActionResult<Warehouse>> Create([FromBody] Warehouse warehouse)
    {
        var created = await _warehouseService.CreateAsync(warehouse);
        await _activityLogService.LogActivityAsync("system", "System", "Create", "Warehouse", created.Id, $"Created warehouse {created.Name}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Warehouse>> Update(int id, [FromBody] Warehouse warehouse)
    {
        if (id != warehouse.Id)
            return BadRequest();
        var updated = await _warehouseService.UpdateAsync(warehouse);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _warehouseService.DeleteAsync(id);
        return NoContent();
    }
}
