using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UpworkERP.Core.Common;
using UpworkERP.Core.Enums;

namespace UpworkERP.Core.Entities.HR;

/// <summary>
/// Employee entity
/// </summary>
public class Employee : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(50)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Position { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Salary { get; set; }

    [Required]
    public DateTime HireDate { get; set; } = DateTime.UtcNow;

    [Required]
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
}
