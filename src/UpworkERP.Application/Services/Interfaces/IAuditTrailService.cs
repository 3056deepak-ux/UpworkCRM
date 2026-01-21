using UpworkERP.Core.Entities.Common;

namespace UpworkERP.Application.Services.Interfaces;

/// <summary>
/// Audit trail service interface
/// </summary>
public interface IAuditTrailService
{
    Task LogAuditAsync(string userId, string userName, string entityType, int entityId, string action, string oldValues, string newValues);
    Task<IEnumerable<AuditTrail>> GetEntityAuditTrailAsync(string entityType, int entityId);
    Task<IEnumerable<AuditTrail>> GetUserAuditTrailAsync(string userId);
}
