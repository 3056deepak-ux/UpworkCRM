using UpworkERP.Core.Entities.Common;
using UpworkERP.Core.Enums;

namespace UpworkERP.Core.Entities.CRM;

/// <summary>
/// Lead entity for lead management
/// </summary>
public class Lead : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public LeadStatus Status { get; set; } = LeadStatus.New;
    public Priority Priority { get; set; } = Priority.Medium;
    public decimal EstimatedValue { get; set; }
    public int? CustomerId { get; set; }
    
    // Navigation property
    public Customer? Customer { get; set; }
}
