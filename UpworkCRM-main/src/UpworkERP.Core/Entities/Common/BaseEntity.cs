using UpworkERP.Core.Interfaces;

namespace UpworkERP.Core.Entities.Common;

/// <summary>
/// Base entity class with audit fields - follows SOLID principles (Single Responsibility)
/// </summary>
public abstract class BaseEntity : IEntity, IAuditable
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
