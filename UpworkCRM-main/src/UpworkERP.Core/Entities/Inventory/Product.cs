using UpworkERP.Core.Entities.Common;
using UpworkERP.Core.Enums;

namespace UpworkERP.Core.Entities.Inventory;

/// <summary>
/// Product entity for inventory management
/// </summary>
public class Product : BaseEntity
{
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int QuantityInStock { get; set; }
    public int ReorderLevel { get; set; }
    public InventoryStatus Status { get; set; } = InventoryStatus.Available;
    
    // Navigation properties
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
