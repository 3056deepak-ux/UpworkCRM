using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.HR;

namespace UpworkERP.API.Controllers;

/// <summary>
/// Controller for PayrollRecord management (HR module)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PayrollController : ControllerBase
{
    private readonly IService<PayrollRecord> _payrollService;
    private readonly IActivityLogService _activityLogService;

    public PayrollController(IService<PayrollRecord> payrollService, IActivityLogService activityLogService)
    {
        _payrollService = payrollService;
        _activityLogService = activityLogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PayrollRecord>>> GetAll()
    {
        var payrollRecords = await _payrollService.GetAllAsync();
        return Ok(payrollRecords);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PayrollRecord>> GetById(int id)
    {
        var payrollRecord = await _payrollService.GetByIdAsync(id);
        if (payrollRecord == null)
            return NotFound();
        return Ok(payrollRecord);
    }

    [HttpPost]
    public async Task<ActionResult<PayrollRecord>> Create([FromBody] PayrollRecord payrollRecord)
    {
        var created = await _payrollService.CreateAsync(payrollRecord);
        await _activityLogService.LogActivityAsync("system", "System", "Create", "PayrollRecord", created.Id, $"Created payroll record {created.Id}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PayrollRecord>> Update(int id, [FromBody] PayrollRecord payrollRecord)
    {
        if (id != payrollRecord.Id)
            return BadRequest();
        var updated = await _payrollService.UpdateAsync(payrollRecord);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _payrollService.DeleteAsync(id);
        return NoContent();
    }
}
