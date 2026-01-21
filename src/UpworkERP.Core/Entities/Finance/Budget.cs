using UpworkERP.Core.Entities.Common;
using UpworkERP.Core.Enums;

namespace UpworkERP.Core.Entities.Finance;

/// <summary>
/// Budget entity for financial budgeting
/// </summary>
public class Budget : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal AllocatedAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public BudgetStatus Status { get; set; } = BudgetStatus.Draft;
    public string Department { get; set; } = string.Empty;
}
