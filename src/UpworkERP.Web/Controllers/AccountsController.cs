using Microsoft.AspNetCore.Mvc;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.Finance;

namespace UpworkERP.Web.Controllers;

public class AccountsController : Controller
{
    private readonly IService<Account> _accountService;
    private readonly IActivityLogService _activityLogService;

    public AccountsController(IService<Account> accountService, IActivityLogService activityLogService)
    {
        _accountService = accountService;
        _activityLogService = activityLogService;
    }

    public async Task<IActionResult> Index()
    {
        var accounts = await _accountService.GetAllAsync();
        return View(accounts);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var account = await _accountService.GetByIdAsync(id.Value);
        if (account == null)
            return NotFound();

        return View(account);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AccountName,AccountNumber,Currency,Balance,Type")] Account account)
    {
        if (ModelState.IsValid)
        {
            account.CreatedBy = "System";
            account.CreatedAt = DateTime.UtcNow;
            await _accountService.CreateAsync(account);
            
            await _activityLogService.LogActivityAsync(
                "Create",
                "Account",
                account.Id.ToString(),
                "System",
                account.Id,
                GetClientIpAddress(),
                $"Created account: {account.AccountName}");
            
            return RedirectToAction(nameof(Index));
        }
        return View(account);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var account = await _accountService.GetByIdAsync(id.Value);
        if (account == null)
            return NotFound();

        return View(account);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,AccountName,AccountNumber,Currency,Balance,Type,CreatedAt,CreatedBy")] Account account)
    {
        if (id != account.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            account.UpdatedBy = "System";
            account.UpdatedAt = DateTime.UtcNow;
            await _accountService.UpdateAsync(account);
            
            await _activityLogService.LogActivityAsync(
                "Update",
                "Account",
                account.Id.ToString(),
                "System",
                account.Id,
                GetClientIpAddress(),
                $"Updated account: {account.AccountName}");
            
            return RedirectToAction(nameof(Index));
        }
        return View(account);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var account = await _accountService.GetByIdAsync(id.Value);
        if (account == null)
            return NotFound();

        return View(account);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _accountService.DeleteAsync(id);
        
        await _activityLogService.LogActivityAsync(
            "Delete",
            "Account",
            id.ToString(),
            "System",
            id,
            GetClientIpAddress(),
            $"Deleted account with ID: {id}");
        
        return RedirectToAction(nameof(Index));
    }

    private string? GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
