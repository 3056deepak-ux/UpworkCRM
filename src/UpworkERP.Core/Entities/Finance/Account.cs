using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UpworkERP.Core.Common;
using UpworkERP.Core.Enums;

namespace UpworkERP.Core.Entities.Finance;

/// <summary>
/// Account entity
/// </summary>
public class Account : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string AccountName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string AccountNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string Currency { get; set; } = "USD";

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; }

    [Required]
    public AccountType Type { get; set; } = AccountType.Asset;
}
