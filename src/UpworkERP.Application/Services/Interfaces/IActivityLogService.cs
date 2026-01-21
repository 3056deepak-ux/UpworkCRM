namespace UpworkERP.Application.Services.Interfaces;

/// <summary>
/// Activity log service interface for tracking user activities
/// </summary>
public interface IActivityLogService
{
    Task LogActivityAsync(
        string action,
        string entityName,
        string entityId,
        string userId,
        int? recordId = null,
        string? ipAddress = null,
        string? details = null);
}
