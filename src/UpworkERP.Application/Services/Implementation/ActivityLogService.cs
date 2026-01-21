using UpworkERP.Core.Entities.Common;
using UpworkERP.Core.Interfaces;
using UpworkERP.Core.Enums;
using UpworkERP.Application.Services.Interfaces;

namespace UpworkERP.Application.Services.Implementation;

/// <summary>
/// Activity logging service implementation
/// </summary>
public class ActivityLogService : IActivityLogService
{
    private readonly IRepository<ActivityLog> _repository;

    public ActivityLogService(IRepository<ActivityLog> repository)
    {
        _repository = repository;
    }

    public async Task LogActivityAsync(string userId, string userName, string activityType, string entityType, int? entityId, string description, string ipAddress)
    {
        var activityLog = new ActivityLog
        {
            UserId = userId,
            UserName = userName,
            ActivityType = Enum.Parse<ActivityType>(activityType),
            EntityType = entityType,
            EntityId = entityId,
            Description = description,
            IPAddress = ipAddress,
            Timestamp = DateTime.UtcNow
        };

        await _repository.AddAsync(activityLog);
        await _repository.SaveChangesAsync();
    }

    public async Task<IEnumerable<ActivityLog>> GetUserActivitiesAsync(string userId)
    {
        return await _repository.FindAsync(al => al.UserId == userId);
    }

    public async Task<IEnumerable<ActivityLog>> GetRecentActivitiesAsync(int count)
    {
        var allActivities = await _repository.GetAllAsync();
        return allActivities.OrderByDescending(al => al.Timestamp).Take(count);
    }
}
