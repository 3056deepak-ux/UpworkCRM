using Microsoft.AspNetCore.Mvc;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.Projects;

namespace UpworkERP.Web.Controllers;

public class ProjectsController : Controller
{
    private readonly IService<Project> _projectService;
    private readonly IActivityLogService _activityLogService;

    public ProjectsController(IService<Project> projectService, IActivityLogService activityLogService)
    {
        _projectService = projectService;
        _activityLogService = activityLogService;
    }

    public async Task<IActionResult> Index()
    {
        var projects = await _projectService.GetAllAsync();
        return View(projects);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var project = await _projectService.GetByIdAsync(id.Value);
        if (project == null)
            return NotFound();

        return View(project);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description,ProjectManager,StartDate,EndDate,Budget,Status")] Project project)
    {
        if (ModelState.IsValid)
        {
            project.CreatedBy = "System";
            project.CreatedAt = DateTime.UtcNow;
            await _projectService.CreateAsync(project);
            
            await _activityLogService.LogActivityAsync(
                "Create",
                "Project",
                project.Id.ToString(),
                "System",
                project.Id,
                GetClientIpAddress(),
                $"Created project: {project.Name}");
            
            return RedirectToAction(nameof(Index));
        }
        return View(project);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var project = await _projectService.GetByIdAsync(id.Value);
        if (project == null)
            return NotFound();

        return View(project);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ProjectManager,StartDate,EndDate,Budget,Status,CreatedAt,CreatedBy")] Project project)
    {
        if (id != project.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            project.UpdatedBy = "System";
            project.UpdatedAt = DateTime.UtcNow;
            await _projectService.UpdateAsync(project);
            
            await _activityLogService.LogActivityAsync(
                "Update",
                "Project",
                project.Id.ToString(),
                "System",
                project.Id,
                GetClientIpAddress(),
                $"Updated project: {project.Name}");
            
            return RedirectToAction(nameof(Index));
        }
        return View(project);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var project = await _projectService.GetByIdAsync(id.Value);
        if (project == null)
            return NotFound();

        return View(project);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _projectService.DeleteAsync(id);
        
        await _activityLogService.LogActivityAsync(
            "Delete",
            "Project",
            id.ToString(),
            "System",
            id,
            GetClientIpAddress(),
            $"Deleted project with ID: {id}");
        
        return RedirectToAction(nameof(Index));
    }

    private string? GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
