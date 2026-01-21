using UpworkERP.Application.Services.Interfaces;

namespace UpworkERP.Application.Services.Implementations;

/// <summary>
/// Activity log service implementation
/// </summary>
public class ActivityLogService : IActivityLogService
{
    public Task LogActivityAsync(
        string action,
        string entityName,
        string entityId,
        string userId,
        int? recordId = null,
        string? ipAddress = null,
        string? details = null)
    {
        // TODO: Implement activity logging to database
        // For now, just log to console for development
        Console.WriteLine($"Activity: {action} on {entityName} by {userId} from {ipAddress}");
        return Task.CompletedTask;
    }
}
