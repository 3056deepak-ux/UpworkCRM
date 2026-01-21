using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.Inventory;

namespace UpworkERP.API.Controllers;

/// <summary>
/// Controller for StockMovement management (Inventory module)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockMovementsController : ControllerBase
{
    private readonly IService<StockMovement> _stockMovementService;
    private readonly IActivityLogService _activityLogService;

    public StockMovementsController(IService<StockMovement> stockMovementService, IActivityLogService activityLogService)
    {
        _stockMovementService = stockMovementService;
        _activityLogService = activityLogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockMovement>>> GetAll()
    {
        var stockMovements = await _stockMovementService.GetAllAsync();
        return Ok(stockMovements);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StockMovement>> GetById(int id)
    {
        var stockMovement = await _stockMovementService.GetByIdAsync(id);
        if (stockMovement == null)
            return NotFound();
        return Ok(stockMovement);
    }

    [HttpPost]
    public async Task<ActionResult<StockMovement>> Create([FromBody] StockMovement stockMovement)
    {
        var created = await _stockMovementService.CreateAsync(stockMovement);
        await _activityLogService.LogActivityAsync("system", "System", "Create", "StockMovement", created.Id, $"Created stock movement {created.Reference}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<StockMovement>> Update(int id, [FromBody] StockMovement stockMovement)
    {
        if (id != stockMovement.Id)
            return BadRequest();
        var updated = await _stockMovementService.UpdateAsync(stockMovement);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _stockMovementService.DeleteAsync(id);
        return NoContent();
    }
}
