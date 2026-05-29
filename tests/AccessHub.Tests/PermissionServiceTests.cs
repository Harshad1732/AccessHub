using AccessHub.Application.Services;
using AccessHub.Domain.Constants;
using AccessHub.Domain.Entities;
using AccessHub.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AccessHub.Tests;

public class PermissionServiceTests
{
    private static AccessHubDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AccessHubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AccessHubDbContext(options);
    }

    [Fact]
    public async Task SuperAdmin_HasAllPermissions()
    {
        await using var db = CreateContext();
        var perm = new Permission { Id = Guid.NewGuid(), Code = PermissionCodes.InvoicesRead, DisplayName = "Read" };
        db.Permissions.Add(perm);
        db.Users.Add(new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "super@test.com",
            Email = "super@test.com",
            IsSuperAdmin = true,
            FullName = "Super"
        });
        await db.SaveChangesAsync();

        var service = new PermissionService(db);
        var result = await service.GetUserPermissionsAsync(db.Users.First().Id);

        result.Should().Contain(PermissionCodes.InvoicesRead);
    }

    [Fact]
    public async Task UserWithRole_HasRolePermissions()
    {
        await using var db = CreateContext();
        var orgId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var permId = Guid.NewGuid();

        db.Organizations.Add(new Organization { Id = orgId, Name = "Test", Slug = "test" });
        db.Permissions.Add(new Permission { Id = permId, Code = PermissionCodes.InvoicesWrite, DisplayName = "Write" });
        db.OrgRoles.Add(new Role { Id = roleId, OrganizationId = orgId, Name = "Writer" });
        db.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = permId });
        db.Users.Add(new ApplicationUser
        {
            Id = userId,
            UserName = "user@test.com",
            Email = "user@test.com",
            OrganizationId = orgId,
            FullName = "User"
        });
        db.OrgUserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
        await db.SaveChangesAsync();

        var service = new PermissionService(db);
        var result = await service.GetUserPermissionsAsync(userId);

        result.Should().Contain(PermissionCodes.InvoicesWrite);
    }

    [Fact]
    public async Task UserWithoutRole_HasNoPermissions()
    {
        await using var db = CreateContext();
        var userId = Guid.NewGuid();
        db.Users.Add(new ApplicationUser
        {
            Id = userId,
            UserName = "lonely@test.com",
            Email = "lonely@test.com",
            FullName = "Lonely"
        });
        await db.SaveChangesAsync();

        var service = new PermissionService(db);
        var result = await service.GetUserPermissionsAsync(userId);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UserHasPermission_ReturnsTrue_WhenAssigned()
    {
        await using var db = CreateContext();
        var orgId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var permId = Guid.NewGuid();

        db.Organizations.Add(new Organization { Id = orgId, Name = "Test", Slug = "test" });
        db.Permissions.Add(new Permission { Id = permId, Code = PermissionCodes.AuditRead, DisplayName = "Audit" });
        db.OrgRoles.Add(new Role { Id = roleId, OrganizationId = orgId, Name = "Auditor" });
        db.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = permId });
        db.Users.Add(new ApplicationUser { Id = userId, UserName = "a@test.com", Email = "a@test.com", OrganizationId = orgId, FullName = "A" });
        db.OrgUserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
        await db.SaveChangesAsync();

        var service = new PermissionService(db);
        var has = await service.UserHasPermissionAsync(userId, PermissionCodes.AuditRead);

        has.Should().BeTrue();
    }

    [Fact]
    public async Task UserHasPermission_ReturnsFalse_WhenNotAssigned()
    {
        await using var db = CreateContext();
        var userId = Guid.NewGuid();
        db.Users.Add(new ApplicationUser { Id = userId, UserName = "b@test.com", Email = "b@test.com", FullName = "B" });
        await db.SaveChangesAsync();

        var service = new PermissionService(db);
        var has = await service.UserHasPermissionAsync(userId, PermissionCodes.RolesWrite);

        has.Should().BeFalse();
    }
}
