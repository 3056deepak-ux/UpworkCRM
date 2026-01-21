using UpworkERP.Core.Entities.Common;
using UpworkERP.Core.Enums;

namespace UpworkERP.Core.Entities.HR;

/// <summary>
/// Leave request entity for employee leave tracking
/// </summary>
public class LeaveRequest : BaseEntity
{
    public int EmployeeId { get; set; }
    public LeaveType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public string? ApproverComments { get; set; }
    
    // Navigation property
    public Employee Employee { get; set; } = null!;
}
