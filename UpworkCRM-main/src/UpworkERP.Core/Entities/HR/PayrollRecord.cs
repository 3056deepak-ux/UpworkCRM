using UpworkERP.Core.Entities.Common;

namespace UpworkERP.Core.Entities.HR;

/// <summary>
/// Payroll record entity for employee salary tracking
/// </summary>
public class PayrollRecord : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
    public decimal GrossPay { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetPay { get; set; }
    public DateTime PaymentDate { get; set; }
    
    // Navigation property
    public Employee Employee { get; set; } = null!;
}
