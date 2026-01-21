using UpworkERP.Core.Entities.Common;

namespace UpworkERP.Application.Services.Interfaces;

/// <summary>
/// Activity logging service interface
/// </summary>
public interface IActivityLogService
{
    Task LogActivityAsync(string userId, string userName, string activityType, string entityType, int? entityId, string description, string ipAddress);
    Task<IEnumerable<ActivityLog>> GetUserActivitiesAsync(string userId);
    Task<IEnumerable<ActivityLog>> GetRecentActivitiesAsync(int count);
}
