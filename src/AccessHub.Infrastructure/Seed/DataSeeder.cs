using AccessHub.Domain.Constants;
using AccessHub.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AccessHub.Infrastructure.Persistence;

namespace AccessHub.Infrastructure.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AccessHubDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await db.Database.MigrateAsync();

        if (!await db.Permissions.AnyAsync())
        {
            var permissions = new List<Permission>
            {
                new() { Id = Guid.NewGuid(), Code = PermissionCodes.OrganizationsManage, DisplayName = "Manage Organizations", Description = "Create and manage organizations" },
                new() { Id = Guid.NewGuid(), Code = PermissionCodes.UsersRead, DisplayName = "Read Users", Description = "View users" },
                new() { Id = Guid.NewGuid(), Code = PermissionCodes.UsersWrite, DisplayName = "Write Users", Description = "Create and update users" },
                new() { Id = Guid.NewGuid(), Code = PermissionCodes.RolesRead, DisplayName = "Read Roles", Description = "View roles" },
                new() { Id = Guid.NewGuid(), Code = PermissionCodes.RolesWrite, DisplayName = "Write Roles", Description = "Create and update roles" },
                new() { Id = Guid.NewGuid(), Code = PermissionCodes.AuditRead, DisplayName = "Read Audit Log", Description = "View audit events" },
                new() { Id = Guid.NewGuid(), Code = PermissionCodes.InvoicesRead, DisplayName = "Read Invoices", Description = "View invoices" },
                new() { Id = Guid.NewGuid(), Code = PermissionCodes.InvoicesWrite, DisplayName = "Write Invoices", Description = "Create invoices" }
            };
            db.Permissions.AddRange(permissions);
            await db.SaveChangesAsync();
        }

        if (await userManager.Users.AnyAsync())
            return;

        var org = new Organization
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Acme Corp",
            Slug = "acme",
            IsActive = true
        };
        db.Organizations.Add(org);

        var superAdmin = new ApplicationUser
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            UserName = "superadmin@accesshub.local",
            Email = "superadmin@accesshub.local",
            EmailConfirmed = true,
            FullName = "Super Admin",
            IsSuperAdmin = true,
            IsActive = true
        };
        await userManager.CreateAsync(superAdmin, "SuperAdmin123!");

        var orgAdmin = new ApplicationUser
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            UserName = "admin@acme.local",
            Email = "admin@acme.local",
            EmailConfirmed = true,
            FullName = "Acme Admin",
            OrganizationId = org.Id,
            IsActive = true
        };
        await userManager.CreateAsync(orgAdmin, "Admin123!");

        var viewer = new ApplicationUser
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            UserName = "viewer@acme.local",
            Email = "viewer@acme.local",
            EmailConfirmed = true,
            FullName = "Acme Viewer",
            OrganizationId = org.Id,
            IsActive = true
        };
        await userManager.CreateAsync(viewer, "Viewer123!");

        var allPermissions = await db.Permissions.ToListAsync();
        var adminRole = new Role
        {
            Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            OrganizationId = org.Id,
            Name = "OrgAdmin",
            Description = "Full organization administrator"
        };
        var viewerRole = new Role
        {
            Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            OrganizationId = org.Id,
            Name = "Viewer",
            Description = "Read-only access"
        };
        db.OrgRoles.AddRange(adminRole, viewerRole);

        foreach (var p in allPermissions.Where(p => p.Code != PermissionCodes.OrganizationsManage))
            db.RolePermissions.Add(new RolePermission { RoleId = adminRole.Id, PermissionId = p.Id });

        var readPerms = allPermissions.Where(p =>
            p.Code is PermissionCodes.UsersRead or PermissionCodes.RolesRead
                or PermissionCodes.AuditRead or PermissionCodes.InvoicesRead).ToList();
        foreach (var p in readPerms)
            db.RolePermissions.Add(new RolePermission { RoleId = viewerRole.Id, PermissionId = p.Id });

        db.OrgUserRoles.Add(new UserRole { UserId = orgAdmin.Id, RoleId = adminRole.Id });
        db.OrgUserRoles.Add(new UserRole { UserId = viewer.Id, RoleId = viewerRole.Id });

        db.Invoices.AddRange(
            new Invoice { Id = Guid.NewGuid(), OrganizationId = org.Id, Number = "INV-001", CustomerName = "Contoso Ltd", Amount = 1500m },
            new Invoice { Id = Guid.NewGuid(), OrganizationId = org.Id, Number = "INV-002", CustomerName = "Fabrikam Inc", Amount = 3200.50m });

        await db.SaveChangesAsync();
    }
}
