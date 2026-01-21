using Microsoft.AspNetCore.Mvc;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.CRM;

namespace UpworkERP.Web.Controllers;

public class CustomersController : Controller
{
    private readonly IService<Customer> _customerService;
    private readonly IActivityLogService _activityLogService;

    public CustomersController(IService<Customer> customerService, IActivityLogService activityLogService)
    {
        _customerService = customerService;
        _activityLogService = activityLogService;
    }

    public async Task<IActionResult> Index()
    {
        var customers = await _customerService.GetAllAsync();
        return View(customers);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var customer = await _customerService.GetByIdAsync(id.Value);
        if (customer == null)
            return NotFound();

        return View(customer);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Email,Company,Address,PhoneNumber,Status")] Customer customer)
    {
        if (ModelState.IsValid)
        {
            customer.CreatedBy = "System";
            customer.CreatedAt = DateTime.UtcNow;
            await _customerService.CreateAsync(customer);
            
            await _activityLogService.LogActivityAsync(
                "Create",
                "Customer",
                customer.Id.ToString(),
                "System",
                customer.Id,
                GetClientIpAddress(),
                $"Created customer: {customer.Name}");
            
            return RedirectToAction(nameof(Index));
        }
        return View(customer);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var customer = await _customerService.GetByIdAsync(id.Value);
        if (customer == null)
            return NotFound();

        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Company,Address,PhoneNumber,Status,CreatedAt,CreatedBy")] Customer customer)
    {
        if (id != customer.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            customer.UpdatedBy = "System";
            customer.UpdatedAt = DateTime.UtcNow;
            await _customerService.UpdateAsync(customer);
            
            await _activityLogService.LogActivityAsync(
                "Update",
                "Customer",
                customer.Id.ToString(),
                "System",
                customer.Id,
                GetClientIpAddress(),
                $"Updated customer: {customer.Name}");
            
            return RedirectToAction(nameof(Index));
        }
        return View(customer);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var customer = await _customerService.GetByIdAsync(id.Value);
        if (customer == null)
            return NotFound();

        return View(customer);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _customerService.DeleteAsync(id);
        
        await _activityLogService.LogActivityAsync(
            "Delete",
            "Customer",
            id.ToString(),
            "System",
            id,
            GetClientIpAddress(),
            $"Deleted customer with ID: {id}");
        
        return RedirectToAction(nameof(Index));
    }

    private string? GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
