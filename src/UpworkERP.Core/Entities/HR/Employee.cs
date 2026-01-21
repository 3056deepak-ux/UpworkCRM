using UpworkERP.Core.Entities.Common;
using UpworkERP.Core.Enums;

namespace UpworkERP.Core.Entities.HR;

/// <summary>
/// Employee entity representing HR employees
/// </summary>
public class Employee : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
    public EmploymentStatus Status { get; set; } = EmploymentStatus.Active;
    
    // Navigation properties
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<PayrollRecord> PayrollRecords { get; set; } = new List<PayrollRecord>();
}
