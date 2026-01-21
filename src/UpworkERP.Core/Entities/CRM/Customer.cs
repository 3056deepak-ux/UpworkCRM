using UpworkERP.Core.Entities.Common;
using UpworkERP.Core.Enums;

namespace UpworkERP.Core.Entities.CRM;

/// <summary>
/// Customer entity for CRM
/// </summary>
public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public CustomerStatus Status { get; set; } = CustomerStatus.Active;
    
    // Navigation properties
    public ICollection<Lead> Leads { get; set; } = new List<Lead>();
}
