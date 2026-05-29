using AccessHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccessHub.Application.Interfaces;

public interface IAccessHubDbContext
{
    DbSet<Organization> Organizations { get; }
    DbSet<ApplicationUser> Users { get; }
    DbSet<Role> OrgRoles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<UserRole> OrgUserRoles { get; }
    DbSet<AuditEvent> AuditEvents { get; }
    DbSet<Invoice> Invoices { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
