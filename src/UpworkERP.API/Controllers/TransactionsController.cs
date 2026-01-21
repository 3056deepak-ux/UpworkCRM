using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.Finance;

namespace UpworkERP.API.Controllers;

/// <summary>
/// Controller for Transaction management (Finance module)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly IService<Transaction> _transactionService;
    private readonly IActivityLogService _activityLogService;

    public TransactionsController(IService<Transaction> transactionService, IActivityLogService activityLogService)
    {
        _transactionService = transactionService;
        _activityLogService = activityLogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetAll()
    {
        var transactions = await _transactionService.GetAllAsync();
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Transaction>> GetById(int id)
    {
        var transaction = await _transactionService.GetByIdAsync(id);
        if (transaction == null)
            return NotFound();
        return Ok(transaction);
    }

    [HttpPost]
    public async Task<ActionResult<Transaction>> Create([FromBody] Transaction transaction)
    {
        var created = await _transactionService.CreateAsync(transaction);
        await _activityLogService.LogActivityAsync("system", "System", "Create", "Transaction", created.Id, $"Created transaction {created.Reference}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Transaction>> Update(int id, [FromBody] Transaction transaction)
    {
        if (id != transaction.Id)
            return BadRequest();
        var updated = await _transactionService.UpdateAsync(transaction);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _transactionService.DeleteAsync(id);
        return NoContent();
    }
}
