using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.CRM;

namespace UpworkERP.API.Controllers;

/// <summary>
/// Controller for Customer management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly IService<Customer> _customerService;
    private readonly IActivityLogService _activityLogService;

    public CustomersController(IService<Customer> customerService, IActivityLogService activityLogService)
    {
        _customerService = customerService;
        _activityLogService = activityLogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
    {
        var customers = await _customerService.GetAllAsync();
        await _activityLogService.LogActivityAsync("system", "System", "Read", "Customer", null, "Retrieved all customers", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetById(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null)
            return NotFound();

        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> Create([FromBody] Customer customer)
    {
        var created = await _customerService.CreateAsync(customer);
        await _activityLogService.LogActivityAsync("system", "System", "Create", "Customer", created.Id, $"Created customer {created.Id}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Customer>> Update(int id, [FromBody] Customer customer)
    {
        if (id != customer.Id)
            return BadRequest();

        var updated = await _customerService.UpdateAsync(customer);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _customerService.DeleteAsync(id);
        return NoContent();
    }
}
