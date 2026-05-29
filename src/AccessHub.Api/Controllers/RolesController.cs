using AccessHub.Api.Services;
using AccessHub.Application.DTOs;
using AccessHub.Application.Interfaces;
using AccessHub.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccessHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController(
    IAccessHubDbContext db,
    IAuditService audit,
    CurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Domain.Constants.PermissionCodes.RolesRead)]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll([FromQuery] Guid organizationId, CancellationToken ct)
    {
        if (!CanAccessOrganization(organizationId)) return Forbid();

        var roles = await db.OrgRoles.AsNoTracking()
            .Where(r => r.OrganizationId == organizationId)
            .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .OrderBy(r => r.Name)
            .ToListAsync(ct);

        return Ok(roles.Select(MapRole));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = Domain.Constants.PermissionCodes.RolesRead)]
    public async Task<ActionResult<RoleDto>> GetById(Guid id, CancellationToken ct)
    {
        var role = await db.OrgRoles.AsNoTracking()
            .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        if (role is null) return NotFound();
        if (!CanAccessOrganization(role.OrganizationId)) return Forbid();
        return Ok(MapRole(role));
    }

    [HttpPost]
    [Authorize(Policy = Domain.Constants.PermissionCodes.RolesWrite)]
    public async Task<ActionResult<RoleDto>> Create([FromQuery] Guid organizationId, [FromBody] CreateRoleRequest request, CancellationToken ct)
    {
        if (!CanAccessOrganization(organizationId)) return Forbid();

        if (await db.OrgRoles.AnyAsync(r => r.OrganizationId == organizationId && r.Name == request.Name, ct))
            return Conflict(new { message = "Role name already exists in this organization." });

        var role = new Role
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            Name = request.Name,
            Description = request.Description
        };

        db.OrgRoles.Add(role);
        await SyncPermissionsAsync(role, request.PermissionCodes, ct);
        await audit.LogAsync(currentUser.UserId, organizationId, "Created", "Role", role.Id.ToString(),
            new { role.Name, request.PermissionCodes }, ct);

        role = await db.OrgRoles.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .FirstAsync(r => r.Id == role.Id, ct);
        return CreatedAtAction(nameof(GetById), new { id = role.Id }, MapRole(role));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Domain.Constants.PermissionCodes.RolesWrite)]
    public async Task<ActionResult<RoleDto>> Update(Guid id, [FromBody] UpdateRoleRequest request, CancellationToken ct)
    {
        var role = await db.OrgRoles.Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id, ct);
        if (role is null) return NotFound();
        if (!CanAccessOrganization(role.OrganizationId)) return Forbid();

        role.Name = request.Name;
        role.Description = request.Description;
        db.RolePermissions.RemoveRange(role.RolePermissions);
        await SyncPermissionsAsync(role, request.PermissionCodes, ct);

        await audit.LogAsync(currentUser.UserId, role.OrganizationId, "Updated", "Role", role.Id.ToString(),
            new { role.Name, request.PermissionCodes }, ct);

        role = await db.OrgRoles.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .FirstAsync(r => r.Id == role.Id, ct);
        return Ok(MapRole(role));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Domain.Constants.PermissionCodes.RolesWrite)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var role = await db.OrgRoles.FindAsync([id], ct);
        if (role is null) return NotFound();
        if (!CanAccessOrganization(role.OrganizationId)) return Forbid();

        db.OrgRoles.Remove(role);
        await db.SaveChangesAsync(ct);

        await audit.LogAsync(currentUser.UserId, role.OrganizationId, "Deleted", "Role", role.Id.ToString(),
            new { role.Name }, ct);

        return NoContent();
    }

    private async Task SyncPermissionsAsync(Role role, IReadOnlyList<string> permissionCodes, CancellationToken ct)
    {
        var permissions = await db.Permissions
            .Where(p => permissionCodes.Contains(p.Code))
            .ToListAsync(ct);

        foreach (var p in permissions)
            db.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = p.Id });

        await db.SaveChangesAsync(ct);
    }

    private static RoleDto MapRole(Role role) => new(
        role.Id,
        role.OrganizationId,
        role.Name,
        role.Description,
        role.RolePermissions.Select(rp => rp.Permission.Code).ToList());

    private bool CanAccessOrganization(Guid organizationId) =>
        currentUser.IsSuperAdmin || currentUser.OrganizationId == organizationId;
}
