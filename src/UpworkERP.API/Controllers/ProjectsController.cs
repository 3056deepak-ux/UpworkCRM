using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.Projects;

namespace UpworkERP.API.Controllers;

/// <summary>
/// Controller for Project management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IService<Project> _projectService;
    private readonly IActivityLogService _activityLogService;

    public ProjectsController(IService<Project> projectService, IActivityLogService activityLogService)
    {
        _projectService = projectService;
        _activityLogService = activityLogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetAll()
    {
        var projects = await _projectService.GetAllAsync();
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetById(int id)
    {
        var project = await _projectService.GetByIdAsync(id);
        if (project == null)
            return NotFound();

        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<Project>> Create([FromBody] Project project)
    {
        var created = await _projectService.CreateAsync(project);
        await _activityLogService.LogActivityAsync("system", "System", "Create", "Project", created.Id, $"Created project {created.Id}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Project>> Update(int id, [FromBody] Project project)
    {
        if (id != project.Id)
            return BadRequest();

        var updated = await _projectService.UpdateAsync(project);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _projectService.DeleteAsync(id);
        return NoContent();
    }
}
