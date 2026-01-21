using UpworkERP.Core.Entities.Common;
using UpworkERP.Core.Interfaces;
using UpworkERP.Application.Services.Interfaces;

namespace UpworkERP.Application.Services.Implementation;

/// <summary>
/// Audit trail service implementation
/// </summary>
public class AuditTrailService : IAuditTrailService
{
    private readonly IRepository<AuditTrail> _repository;

    public AuditTrailService(IRepository<AuditTrail> repository)
    {
        _repository = repository;
    }

    public async Task LogAuditAsync(string userId, string userName, string entityType, int entityId, string action, string oldValues, string newValues)
    {
        var auditTrail = new AuditTrail
        {
            UserId = userId,
            UserName = userName,
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            OldValues = oldValues,
            NewValues = newValues,
            Timestamp = DateTime.UtcNow
        };

        await _repository.AddAsync(auditTrail);
        await _repository.SaveChangesAsync();
    }

    public async Task<IEnumerable<AuditTrail>> GetEntityAuditTrailAsync(string entityType, int entityId)
    {
        return await _repository.FindAsync(at => at.EntityType == entityType && at.EntityId == entityId);
    }

    public async Task<IEnumerable<AuditTrail>> GetUserAuditTrailAsync(string userId)
    {
        return await _repository.FindAsync(at => at.UserId == userId);
    }
}
