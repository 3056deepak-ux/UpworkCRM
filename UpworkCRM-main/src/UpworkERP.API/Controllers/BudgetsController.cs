using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.Finance;

namespace UpworkERP.API.Controllers;

/// <summary>
/// Controller for Budget management (Finance module)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BudgetsController : ControllerBase
{
    private readonly IService<Budget> _budgetService;
    private readonly IActivityLogService _activityLogService;

    public BudgetsController(IService<Budget> budgetService, IActivityLogService activityLogService)
    {
        _budgetService = budgetService;
        _activityLogService = activityLogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Budget>>> GetAll()
    {
        var budgets = await _budgetService.GetAllAsync();
        return Ok(budgets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Budget>> GetById(int id)
    {
        var budget = await _budgetService.GetByIdAsync(id);
        if (budget == null)
            return NotFound();
        return Ok(budget);
    }

    [HttpPost]
    public async Task<ActionResult<Budget>> Create([FromBody] Budget budget)
    {
        var created = await _budgetService.CreateAsync(budget);
        await _activityLogService.LogActivityAsync("system", "System", "Create", "Budget", created.Id, $"Created budget {created.Name}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Budget>> Update(int id, [FromBody] Budget budget)
    {
        if (id != budget.Id)
            return BadRequest();
        var updated = await _budgetService.UpdateAsync(budget);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _budgetService.DeleteAsync(id);
        return NoContent();
    }
}
