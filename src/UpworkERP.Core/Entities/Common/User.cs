using UpworkERP.Core.Entities.Common;
using UpworkERP.Core.Enums;

namespace UpworkERP.Core.Entities.Common;

/// <summary>
/// User entity for authentication and authorization
/// </summary>
public class User : BaseEntity
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Employee;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginDate { get; set; }
    
    // Navigation properties for RBAC
    public ICollection<Auth.UserRole> UserRoles { get; set; } = new List<Auth.UserRole>();
}
