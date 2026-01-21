using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.CRM;

namespace UpworkERP.API.Controllers;

/// <summary>
/// Controller for Lead management (CRM module)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeadsController : ControllerBase
{
    private readonly IService<Lead> _leadService;
    private readonly IActivityLogService _activityLogService;

    public LeadsController(IService<Lead> leadService, IActivityLogService activityLogService)
    {
        _leadService = leadService;
        _activityLogService = activityLogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Lead>>> GetAll()
    {
        var leads = await _leadService.GetAllAsync();
        return Ok(leads);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Lead>> GetById(int id)
    {
        var lead = await _leadService.GetByIdAsync(id);
        if (lead == null)
            return NotFound();
        return Ok(lead);
    }

    [HttpPost]
    public async Task<ActionResult<Lead>> Create([FromBody] Lead lead)
    {
        var created = await _leadService.CreateAsync(lead);
        await _activityLogService.LogActivityAsync("system", "System", "Create", "Lead", created.Id, $"Created lead {created.Name}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Lead>> Update(int id, [FromBody] Lead lead)
    {
        if (id != lead.Id)
            return BadRequest();
        var updated = await _leadService.UpdateAsync(lead);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _leadService.DeleteAsync(id);
        return NoContent();
    }
}
