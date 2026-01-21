using UpworkERP.Core.Entities.Auth;
using UpworkERP.Core.Interfaces;
using UpworkERP.Application.Services.Interfaces;

namespace UpworkERP.Application.Services.Implementation;

/// <summary>
/// Authorization service implementation for RBAC
/// </summary>
public class AuthorizationService : IAuthorizationService
{
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IRepository<RolePermission> _rolePermissionRepository;
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IRepository<Role> _roleRepository;

    public AuthorizationService(
        IRepository<UserRole> userRoleRepository,
        IRepository<RolePermission> rolePermissionRepository,
        IRepository<Permission> permissionRepository,
        IRepository<Role> roleRepository)
    {
        _userRoleRepository = userRoleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _permissionRepository = permissionRepository;
        _roleRepository = roleRepository;
    }

    public async Task<bool> UserHasPermissionAsync(int userId, string module, string action)
    {
        var userRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == userId);
        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

        if (!roleIds.Any())
            return false;

        var rolePermissions = await _rolePermissionRepository.FindAsync(rp => roleIds.Contains(rp.RoleId));
        var permissionIds = rolePermissions.Select(rp => rp.PermissionId).ToList();

        if (!permissionIds.Any())
            return false;

        var permissions = await _permissionRepository.FindAsync(p => 
            permissionIds.Contains(p.Id) && 
            p.Module == module && 
            p.Action == action);

        return permissions.Any();
    }

    public async Task<IEnumerable<Role>> GetUserRolesAsync(int userId)
    {
        var userRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == userId);
        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

        var roles = await _roleRepository.FindAsync(r => roleIds.Contains(r.Id));
        return roles;
    }

    public async Task<IEnumerable<Permission>> GetUserPermissionsAsync(int userId)
    {
        var userRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == userId);
        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

        var rolePermissions = await _rolePermissionRepository.FindAsync(rp => roleIds.Contains(rp.RoleId));
        var permissionIds = rolePermissions.Select(rp => rp.PermissionId).Distinct().ToList();

        var permissions = await _permissionRepository.FindAsync(p => permissionIds.Contains(p.Id));
        return permissions;
    }

    public async Task AssignRoleToUserAsync(int userId, int roleId)
    {
        var existingUserRole = await _userRoleRepository.FindAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        if (existingUserRole.Any())
            return;

        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId
        };

        await _userRoleRepository.AddAsync(userRole);
        await _userRoleRepository.SaveChangesAsync();
    }

    public async Task RemoveRoleFromUserAsync(int userId, int roleId)
    {
        var userRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        foreach (var userRole in userRoles)
        {
            await _userRoleRepository.DeleteAsync(userRole);
        }
        await _userRoleRepository.SaveChangesAsync();
    }

    public async Task AssignPermissionToRoleAsync(int roleId, int permissionId)
    {
        var existingRolePermission = await _rolePermissionRepository.FindAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
        if (existingRolePermission.Any())
            return;

        var rolePermission = new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId
        };

        await _rolePermissionRepository.AddAsync(rolePermission);
        await _rolePermissionRepository.SaveChangesAsync();
    }

    public async Task RemovePermissionFromRoleAsync(int roleId, int permissionId)
    {
        var rolePermissions = await _rolePermissionRepository.FindAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
        foreach (var rolePermission in rolePermissions)
        {
            await _rolePermissionRepository.DeleteAsync(rolePermission);
        }
        await _rolePermissionRepository.SaveChangesAsync();
    }
}
