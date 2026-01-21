using UpworkERP.Core.Entities.Common;

namespace UpworkERP.Core.Entities.Inventory;

/// <summary>
/// Warehouse entity for warehouse management
/// </summary>
public class Warehouse : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string ManagerName { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
