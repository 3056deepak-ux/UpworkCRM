using System.ComponentModel.DataAnnotations;
using UpworkERP.Core.Common;
using UpworkERP.Core.Enums;

namespace UpworkERP.Core.Entities.CRM;

/// <summary>
/// Customer entity
/// </summary>
public class Customer : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Company { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(50)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public CustomerStatus Status { get; set; } = CustomerStatus.Active;
}
