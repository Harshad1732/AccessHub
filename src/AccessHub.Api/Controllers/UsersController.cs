using AccessHub.Api.Services;
using AccessHub.Application.DTOs;
using AccessHub.Application.Interfaces;
using AccessHub.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccessHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(
    UserManager<ApplicationUser> userManager,
    IAccessHubDbContext db,
    IAuditService audit,
    CurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Domain.Constants.PermissionCodes.UsersRead)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll([FromQuery] Guid? organizationId, CancellationToken ct)
    {
        var orgId = ResolveOrganizationId(organizationId);
        if (orgId is null) return Forbid();

        var users = await GetUserDtos(orgId.Value, ct);
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = Domain.Constants.PermissionCodes.UsersRead)]
    public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken ct)
    {
        var dto = await BuildUserDto(id, ct);
        if (dto is null) return NotFound();
        if (!CanAccessOrganization(dto.OrganizationId)) return Forbid();
        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Policy = Domain.Constants.PermissionCodes.UsersWrite)]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        if (!CanAccessOrganization(request.OrganizationId))
            return Forbid();

        if (await userManager.FindByEmailAsync(request.Email) is not null)
            return Conflict(new { message = "Email already exists." });

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true,
            FullName = request.FullName,
            OrganizationId = request.OrganizationId,
            IsActive = true
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(new { message = string.Join("; ", result.Errors.Select(e => e.Description)) });

        await AssignRolesAsync(user.Id, request.OrganizationId, request.RoleIds, ct);
        await audit.LogAsync(currentUser.UserId, request.OrganizationId, "Created", "User", user.Id.ToString(),
            new { user.Email, user.FullName }, ct);

        var dto = await BuildUserDto(user.Id, ct);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Domain.Constants.PermissionCodes.UsersWrite)]
    public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        var user = await userManager.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user is null) return NotFound();
        if (!CanAccessOrganization(user.OrganizationId)) return Forbid();

        user.FullName = request.FullName;
        user.IsActive = request.IsActive;

        db.OrgUserRoles.RemoveRange(user.UserRoles);
        await userManager.UpdateAsync(user);

        if (user.OrganizationId.HasValue && request.RoleIds is not null)
            await AssignRolesAsync(user.Id, user.OrganizationId.Value, request.RoleIds, ct);

        await audit.LogAsync(currentUser.UserId, user.OrganizationId, "Updated", "User", user.Id.ToString(),
            new { user.FullName, user.IsActive, request.RoleIds }, ct);

        var dto = await BuildUserDto(id, ct);
        return Ok(dto);
    }

    private async Task AssignRolesAsync(Guid userId, Guid organizationId, IReadOnlyList<Guid>? roleIds, CancellationToken ct)
    {
        if (roleIds is null || roleIds.Count == 0) return;

        var validRoleIds = await db.OrgRoles
            .Where(r => r.OrganizationId == organizationId && roleIds.Contains(r.Id))
            .Select(r => r.Id)
            .ToListAsync(ct);

        foreach (var roleId in validRoleIds)
            db.OrgUserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });

        await db.SaveChangesAsync(ct);
    }

    private async Task<List<UserDto>> GetUserDtos(Guid organizationId, CancellationToken ct)
    {
        var users = await userManager.Users.AsNoTracking()
            .Where(u => u.OrganizationId == organizationId)
            .ToListAsync(ct);

        var result = new List<UserDto>();
        foreach (var u in users)
        {
            var dto = await BuildUserDto(u.Id, ct);
            if (dto is not null) result.Add(dto);
        }
        return result;
    }

    private async Task<UserDto?> BuildUserDto(Guid id, CancellationToken ct)
    {
        var user = await userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user is null) return null;

        var roleIds = await db.OrgUserRoles.AsNoTracking()
            .Where(ur => ur.UserId == id)
            .Select(ur => ur.RoleId)
            .ToListAsync(ct);

        return new UserDto(user.Id, user.Email!, user.FullName, user.OrganizationId, user.IsActive, user.IsSuperAdmin, roleIds);
    }

    private Guid? ResolveOrganizationId(Guid? requested)
    {
        if (currentUser.IsSuperAdmin)
            return requested ?? currentUser.OrganizationId;
        return currentUser.OrganizationId;
    }

    private bool CanAccessOrganization(Guid? organizationId) =>
        currentUser.IsSuperAdmin || currentUser.OrganizationId == organizationId;
}
