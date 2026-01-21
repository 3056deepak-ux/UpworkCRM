using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UpworkERP.Core.Common;
using UpworkERP.Core.Enums;

namespace UpworkERP.Core.Entities.Inventory;

/// <summary>
/// Product entity
/// </summary>
public class Product : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string SKU { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Required]
    public int QuantityInStock { get; set; }

    [Required]
    public int ReorderLevel { get; set; }

    [Required]
    public ProductStatus Status { get; set; } = ProductStatus.Active;
}
