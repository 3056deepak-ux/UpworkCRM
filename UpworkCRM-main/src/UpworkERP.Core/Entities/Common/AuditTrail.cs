using UpworkERP.Core.Entities.Common;

namespace UpworkERP.Core.Entities.Common;

/// <summary>
/// Audit trail entity for tracking data changes
/// </summary>
public class AuditTrail : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string OldValues { get; set; } = string.Empty;
    public string NewValues { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
