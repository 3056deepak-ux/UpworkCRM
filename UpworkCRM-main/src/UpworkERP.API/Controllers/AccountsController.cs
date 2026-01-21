using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.Finance;

namespace UpworkERP.API.Controllers;

/// <summary>
/// Controller for Account management (Finance module)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IService<Account> _accountService;
    private readonly IActivityLogService _activityLogService;

    public AccountsController(IService<Account> accountService, IActivityLogService activityLogService)
    {
        _accountService = accountService;
        _activityLogService = activityLogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Account>>> GetAll()
    {
        var accounts = await _accountService.GetAllAsync();
        await _activityLogService.LogActivityAsync("system", "System", "Read", "Account", null, "Retrieved all accounts", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return Ok(accounts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Account>> GetById(int id)
    {
        var account = await _accountService.GetByIdAsync(id);
        if (account == null)
            return NotFound();
        return Ok(account);
    }

    [HttpPost]
    public async Task<ActionResult<Account>> Create([FromBody] Account account)
    {
        var created = await _accountService.CreateAsync(account);
        await _activityLogService.LogActivityAsync("system", "System", "Create", "Account", created.Id, $"Created account {created.AccountNumber}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Account>> Update(int id, [FromBody] Account account)
    {
        if (id != account.Id)
            return BadRequest();
        var updated = await _accountService.UpdateAsync(account);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _accountService.DeleteAsync(id);
        return NoContent();
    }
}
