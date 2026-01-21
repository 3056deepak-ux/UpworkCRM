using UpworkERP.Core.Entities.Common;

namespace UpworkERP.Core.Entities.Auth;

/// <summary>
/// Permission entity for RBAC
/// </summary>
public class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
