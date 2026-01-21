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

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var product = await _productService.GetByIdAsync(id.Value);
        if (product == null)
            return NotFound();

        return View(product);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description,Category,SKU,UnitPrice,QuantityInStock,ReorderLevel,Status")] Product product)
    {
        if (ModelState.IsValid)
        {
            product.CreatedBy = "System";
            product.CreatedAt = DateTime.UtcNow;
            await _productService.CreateAsync(product);
            
            await _activityLogService.LogActivityAsync(
                "Create",
                "Product",
                product.Id.ToString(),
                "System",
                product.Id,
                GetClientIpAddress(),
                $"Created product: {product.Name}");
            
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var product = await _productService.GetByIdAsync(id.Value);
        if (product == null)
            return NotFound();

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Category,SKU,UnitPrice,QuantityInStock,ReorderLevel,Status,CreatedAt,CreatedBy")] Product product)
    {
        if (id != product.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            product.UpdatedBy = "System";
            product.UpdatedAt = DateTime.UtcNow;
            await _productService.UpdateAsync(product);
            
            await _activityLogService.LogActivityAsync(
                "Update",
                "Product",
                product.Id.ToString(),
                "System",
                product.Id,
                GetClientIpAddress(),
                $"Updated product: {product.Name}");
            
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var product = await _productService.GetByIdAsync(id.Value);
        if (product == null)
            return NotFound();

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _productService.DeleteAsync(id);
        
        await _activityLogService.LogActivityAsync(
            "Delete",
            "Product",
            id.ToString(),
            "System",
            id,
            GetClientIpAddress(),
            $"Deleted product with ID: {id}");
        
        return RedirectToAction(nameof(Index));
    }

    private string? GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
