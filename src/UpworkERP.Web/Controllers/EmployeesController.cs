using Microsoft.AspNetCore.Mvc;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.HR;

namespace UpworkERP.Web.Controllers;

public class EmployeesController : Controller
{
    private readonly IService<Employee> _employeeService;
    private readonly IActivityLogService _activityLogService;

    public EmployeesController(IService<Employee> employeeService, IActivityLogService activityLogService)
    {
        _employeeService = employeeService;
        _activityLogService = activityLogService;
    }

    // GET: Employees
    public async Task<IActionResult> Index()
    {
        var employees = await _employeeService.GetAllAsync();
        await _activityLogService.LogActivityAsync("system", "System", "Read", "Employee", null, 
            "Viewed employees list", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return View(employees);
    }

    // GET: Employees/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }
        return View(employee);
    }

    // GET: Employees/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Employees/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee employee)
    {
        if (ModelState.IsValid)
        {
            await _employeeService.CreateAsync(employee);
            await _activityLogService.LogActivityAsync("system", "System", "Create", "Employee", employee.Id,
                $"Created employee {employee.FirstName} {employee.LastName}", 
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            return RedirectToAction(nameof(Index));
        }
        return View(employee);
    }

    // GET: Employees/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }
        return View(employee);
    }

    // POST: Employees/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Employee employee)
    {
        if (id != employee.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _employeeService.UpdateAsync(employee);
            await _activityLogService.LogActivityAsync("system", "System", "Update", "Employee", employee.Id,
                $"Updated employee {employee.FirstName} {employee.LastName}", 
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            return RedirectToAction(nameof(Index));
        }
        return View(employee);
    }

    // GET: Employees/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }
        return View(employee);
    }

    // POST: Employees/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        await _employeeService.DeleteAsync(id);
        await _activityLogService.LogActivityAsync("system", "System", "Delete", "Employee", id,
            $"Deleted employee {employee?.FirstName} {employee?.LastName}", 
            HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return RedirectToAction(nameof(Index));
    }
}
