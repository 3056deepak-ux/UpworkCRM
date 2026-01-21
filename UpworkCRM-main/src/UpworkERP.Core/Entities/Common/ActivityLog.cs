using UpworkERP.Core.Entities.Common;
using UpworkERP.Core.Enums;

namespace UpworkERP.Core.Entities.Common;

/// <summary>
/// Activity log entity for tracking user activities
/// </summary>
public class ActivityLog : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public ActivityType ActivityType { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
