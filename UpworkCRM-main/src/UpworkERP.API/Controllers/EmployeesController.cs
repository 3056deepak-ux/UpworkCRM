using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.HR;

namespace UpworkERP.API.Controllers;

/// <summary>
/// Controller for Employee management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IService<Employee> _employeeService;
    private readonly IActivityLogService _activityLogService;

    public EmployeesController(IService<Employee> employeeService, IActivityLogService activityLogService)
    {
        _employeeService = employeeService;
        _activityLogService = activityLogService;
    }

    /// <summary>
    /// Get all employees
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetAll()
    {
        var employees = await _employeeService.GetAllAsync();
        await _activityLogService.LogActivityAsync("system", "System", "Read", "Employee", null, "Retrieved all employees", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return Ok(employees);
    }

    /// <summary>
    /// Get employee by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetById(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
            return NotFound();

        await _activityLogService.LogActivityAsync("system", "System", "Read", "Employee", id, $"Retrieved employee {id}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return Ok(employee);
    }

    /// <summary>
    /// Create new employee
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Employee>> Create([FromBody] Employee employee)
    {
        var created = await _employeeService.CreateAsync(employee);
        await _activityLogService.LogActivityAsync("system", "System", "Create", "Employee", created.Id, $"Created employee {created.Id}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update existing employee
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<Employee>> Update(int id, [FromBody] Employee employee)
    {
        if (id != employee.Id)
            return BadRequest();

        var updated = await _employeeService.UpdateAsync(employee);
        await _activityLogService.LogActivityAsync("system", "System", "Update", "Employee", id, $"Updated employee {id}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return Ok(updated);
    }

    /// <summary>
    /// Delete employee
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _employeeService.DeleteAsync(id);
        await _activityLogService.LogActivityAsync("system", "System", "Delete", "Employee", id, $"Deleted employee {id}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return NoContent();
    }
}
