using UpworkERP.Core.Entities.Common;

namespace UpworkERP.Core.Entities.Auth;

/// <summary>
/// Many-to-many relationship between Role and Permission
/// </summary>
public class RolePermission : BaseEntity
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    
    // Navigation properties
    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
