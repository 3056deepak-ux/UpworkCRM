using UpworkERP.Core.Entities.Common;
using UpworkERP.Core.Enums;

namespace UpworkERP.Core.Entities.Finance;

/// <summary>
/// Transaction entity for financial transactions
/// </summary>
public class Transaction : BaseEntity
{
    public int AccountId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public string Reference { get; set; } = string.Empty;
    
    // Navigation property
    public Account Account { get; set; } = null!;
}
