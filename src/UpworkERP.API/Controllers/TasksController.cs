using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.Projects;

namespace UpworkERP.API.Controllers;

/// <summary>
/// Controller for ProjectTask management (Projects module)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly IService<ProjectTask> _taskService;
    private readonly IActivityLogService _activityLogService;

    public TasksController(IService<ProjectTask> taskService, IActivityLogService activityLogService)
    {
        _taskService = taskService;
        _activityLogService = activityLogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectTask>>> GetAll()
    {
        var tasks = await _taskService.GetAllAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectTask>> GetById(int id)
    {
        var task = await _taskService.GetByIdAsync(id);
        if (task == null)
            return NotFound();
        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectTask>> Create([FromBody] ProjectTask task)
    {
        var created = await _taskService.CreateAsync(task);
        await _activityLogService.LogActivityAsync("system", "System", "Create", "ProjectTask", created.Id, $"Created task {created.Title}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProjectTask>> Update(int id, [FromBody] ProjectTask task)
    {
        if (id != task.Id)
            return BadRequest();
        var updated = await _taskService.UpdateAsync(task);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _taskService.DeleteAsync(id);
        return NoContent();
    }
}
