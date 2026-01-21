using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.Projects;

namespace UpworkERP.API.Controllers;

/// <summary>
/// Controller for TimeEntry management (Projects module)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TimeEntriesController : ControllerBase
{
    private readonly IService<TimeEntry> _timeEntryService;
    private readonly IActivityLogService _activityLogService;

    public TimeEntriesController(IService<TimeEntry> timeEntryService, IActivityLogService activityLogService)
    {
        _timeEntryService = timeEntryService;
        _activityLogService = activityLogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TimeEntry>>> GetAll()
    {
        var timeEntries = await _timeEntryService.GetAllAsync();
        return Ok(timeEntries);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TimeEntry>> GetById(int id)
    {
        var timeEntry = await _timeEntryService.GetByIdAsync(id);
        if (timeEntry == null)
            return NotFound();
        return Ok(timeEntry);
    }

    [HttpPost]
    public async Task<ActionResult<TimeEntry>> Create([FromBody] TimeEntry timeEntry)
    {
        var created = await _timeEntryService.CreateAsync(timeEntry);
        await _activityLogService.LogActivityAsync("system", "System", "Create", "TimeEntry", created.Id, $"Created time entry {created.Id}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TimeEntry>> Update(int id, [FromBody] TimeEntry timeEntry)
    {
        if (id != timeEntry.Id)
            return BadRequest();
        var updated = await _timeEntryService.UpdateAsync(timeEntry);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _timeEntryService.DeleteAsync(id);
        return NoContent();
    }
}
