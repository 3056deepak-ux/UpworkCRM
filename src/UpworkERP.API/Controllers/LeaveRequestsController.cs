using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.HR;

namespace UpworkERP.API.Controllers;

/// <summary>
/// Controller for LeaveRequest management (HR module)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaveRequestsController : ControllerBase
{
    private readonly IService<LeaveRequest> _leaveRequestService;
    private readonly IActivityLogService _activityLogService;

    public LeaveRequestsController(IService<LeaveRequest> leaveRequestService, IActivityLogService activityLogService)
    {
        _leaveRequestService = leaveRequestService;
        _activityLogService = activityLogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LeaveRequest>>> GetAll()
    {
        var leaveRequests = await _leaveRequestService.GetAllAsync();
        return Ok(leaveRequests);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LeaveRequest>> GetById(int id)
    {
        var leaveRequest = await _leaveRequestService.GetByIdAsync(id);
        if (leaveRequest == null)
            return NotFound();
        return Ok(leaveRequest);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveRequest>> Create([FromBody] LeaveRequest leaveRequest)
    {
        var created = await _leaveRequestService.CreateAsync(leaveRequest);
        await _activityLogService.LogActivityAsync("system", "System", "Create", "LeaveRequest", created.Id, $"Created leave request {created.Id}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LeaveRequest>> Update(int id, [FromBody] LeaveRequest leaveRequest)
    {
        if (id != leaveRequest.Id)
            return BadRequest();
        var updated = await _leaveRequestService.UpdateAsync(leaveRequest);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _leaveRequestService.DeleteAsync(id);
        return NoContent();
    }
}
