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

    public async Task<IActionResult> Details(int id)
    {
        var account = await _accountService.GetByIdAsync(id);
        if (account == null) return NotFound();
        return View(account);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Account account)
    {
        if (ModelState.IsValid)
        {
            await _accountService.CreateAsync(account);
            await _activityLogService.LogActivityAsync("system", "System", "Create", "Account", account.Id,
                $"Created account {account.AccountNumber}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            return RedirectToAction(nameof(Index));
        }
        return View(account);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var account = await _accountService.GetByIdAsync(id);
        if (account == null) return NotFound();
        return View(account);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Account account)
    {
        if (id != account.Id) return NotFound();
        if (ModelState.IsValid)
        {
            await _accountService.UpdateAsync(account);
            return RedirectToAction(nameof(Index));
        }
        return View(account);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var account = await _accountService.GetByIdAsync(id);
        if (account == null) return NotFound();
        return View(account);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _accountService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
