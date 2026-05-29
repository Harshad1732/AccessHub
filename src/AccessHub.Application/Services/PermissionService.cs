using AccessHub.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AccessHub.Application.Services;

public class PermissionService(IAccessHubDbContext db) : IPermissionService
{
    public async Task<IReadOnlyList<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await db.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            return [];

        if (user.IsSuperAdmin)
            return await db.Permissions.AsNoTracking()
                .Select(p => p.Code)
                .ToListAsync(cancellationToken);

        return await (
            from ur in db.OrgUserRoles.AsNoTracking()
            where ur.UserId == userId
            join rp in db.RolePermissions.AsNoTracking() on ur.RoleId equals rp.RoleId
            join p in db.Permissions.AsNoTracking() on rp.PermissionId equals p.Id
            select p.Code).Distinct().ToListAsync(cancellationToken);
    }

    public async Task<bool> UserHasPermissionAsync(Guid userId, string permissionCode, CancellationToken cancellationToken = default)
    {
        var permissions = await GetUserPermissionsAsync(userId, cancellationToken);
        return permissions.Contains(permissionCode);
    }
}
