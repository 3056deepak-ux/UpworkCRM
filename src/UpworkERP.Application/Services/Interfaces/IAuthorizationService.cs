using UpworkERP.Core.Entities.Auth;

namespace UpworkERP.Application.Services.Interfaces;

/// <summary>
/// Authorization service interface for RBAC
/// </summary>
public interface IAuthorizationService
{
    Task<bool> UserHasPermissionAsync(int userId, string module, string action);
    Task<IEnumerable<Role>> GetUserRolesAsync(int userId);
    Task<IEnumerable<Permission>> GetUserPermissionsAsync(int userId);
    Task AssignRoleToUserAsync(int userId, int roleId);
    Task RemoveRoleFromUserAsync(int userId, int roleId);
    Task AssignPermissionToRoleAsync(int roleId, int permissionId);
    Task RemovePermissionFromRoleAsync(int roleId, int permissionId);
}
