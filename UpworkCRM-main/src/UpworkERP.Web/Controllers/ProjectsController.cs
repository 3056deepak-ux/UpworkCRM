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

    public async Task<IActionResult> Details(int id)
    {
        var project = await _projectService.GetByIdAsync(id);
        if (project == null) return NotFound();
        return View(project);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Project project)
    {
        if (ModelState.IsValid)
        {
            await _projectService.CreateAsync(project);
            await _activityLogService.LogActivityAsync("system", "System", "Create", "Project", project.Id,
                $"Created project {project.Name}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            return RedirectToAction(nameof(Index));
        }
        return View(project);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var project = await _projectService.GetByIdAsync(id);
        if (project == null) return NotFound();
        return View(project);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Project project)
    {
        if (id != project.Id) return NotFound();
        if (ModelState.IsValid)
        {
            await _projectService.UpdateAsync(project);
            return RedirectToAction(nameof(Index));
        }
        return View(project);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var project = await _projectService.GetByIdAsync(id);
        if (project == null) return NotFound();
        return View(project);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _projectService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
