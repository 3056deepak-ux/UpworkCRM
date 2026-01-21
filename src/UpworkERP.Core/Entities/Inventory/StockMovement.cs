using UpworkERP.Core.Entities.Common;
using UpworkERP.Core.Enums;

namespace UpworkERP.Core.Entities.Inventory;

/// <summary>
/// Stock movement entity for inventory tracking
/// </summary>
public class StockMovement : BaseEntity
{
    public int ProductId { get; set; }
    public int? WarehouseId { get; set; }
    public StockMovementType MovementType { get; set; }
    public int Quantity { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime MovementDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Product Product { get; set; } = null!;
    public Warehouse? Warehouse { get; set; }
}
