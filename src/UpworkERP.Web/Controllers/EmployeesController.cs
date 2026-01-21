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

    public async Task<IActionResult> Index()
    {
        var employees = await _employeeService.GetAllAsync();
        return View(employees);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var employee = await _employeeService.GetByIdAsync(id.Value);
        if (employee == null)
            return NotFound();

        return View(employee);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,PhoneNumber,Department,Position,Salary,HireDate,Status")] Employee employee)
    {
        if (ModelState.IsValid)
        {
            employee.CreatedBy = "System";
            employee.CreatedAt = DateTime.UtcNow;
            await _employeeService.CreateAsync(employee);
            
            await _activityLogService.LogActivityAsync(
                "Create",
                "Employee",
                employee.Id.ToString(),
                "System",
                employee.Id,
                GetClientIpAddress(),
                $"Created employee: {employee.FirstName} {employee.LastName}");
            
            return RedirectToAction(nameof(Index));
        }
        return View(employee);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var employee = await _employeeService.GetByIdAsync(id.Value);
        if (employee == null)
            return NotFound();

        return View(employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumber,Department,Position,Salary,HireDate,Status,CreatedAt,CreatedBy")] Employee employee)
    {
        if (id != employee.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            employee.UpdatedBy = "System";
            employee.UpdatedAt = DateTime.UtcNow;
            await _employeeService.UpdateAsync(employee);
            
            await _activityLogService.LogActivityAsync(
                "Update",
                "Employee",
                employee.Id.ToString(),
                "System",
                employee.Id,
                GetClientIpAddress(),
                $"Updated employee: {employee.FirstName} {employee.LastName}");
            
            return RedirectToAction(nameof(Index));
        }
        return View(employee);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var employee = await _employeeService.GetByIdAsync(id.Value);
        if (employee == null)
            return NotFound();

        return View(employee);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _employeeService.DeleteAsync(id);
        
        await _activityLogService.LogActivityAsync(
            "Delete",
            "Employee",
            id.ToString(),
            "System",
            id,
            GetClientIpAddress(),
            $"Deleted employee with ID: {id}");
        
        return RedirectToAction(nameof(Index));
    }

    private string? GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
