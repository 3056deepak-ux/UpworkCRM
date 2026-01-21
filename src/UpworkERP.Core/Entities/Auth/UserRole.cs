using UpworkERP.Core.Entities.Common;

namespace UpworkERP.Core.Entities.Auth;

/// <summary>
/// Many-to-many relationship between User and Role
/// </summary>
public class UserRole : BaseEntity
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
