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

    public async Task<IActionResult> Details(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null) return NotFound();
        return View(customer);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (ModelState.IsValid)
        {
            await _customerService.CreateAsync(customer);
            await _activityLogService.LogActivityAsync("system", "System", "Create", "Customer", customer.Id,
                $"Created customer {customer.Name}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            return RedirectToAction(nameof(Index));
        }
        return View(customer);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null) return NotFound();
        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer customer)
    {
        if (id != customer.Id) return NotFound();
        if (ModelState.IsValid)
        {
            await _customerService.UpdateAsync(customer);
            return RedirectToAction(nameof(Index));
        }
        return View(customer);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null) return NotFound();
        return View(customer);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _customerService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
