using Microsoft.AspNetCore.Mvc;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.Inventory;

namespace UpworkERP.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IService<Product> _productService;
    private readonly IActivityLogService _activityLogService;

    public ProductsController(IService<Product> productService, IActivityLogService activityLogService)
    {
        _productService = productService;
        _activityLogService = activityLogService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllAsync();
        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            await _productService.CreateAsync(product);
            await _activityLogService.LogActivityAsync("system", "System", "Create", "Product", product.Id,
                $"Created product {product.Name}", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id) return NotFound();
        if (ModelState.IsValid)
        {
            await _productService.UpdateAsync(product);
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _productService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
