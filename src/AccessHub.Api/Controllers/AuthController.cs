using System.Text.RegularExpressions;
using AccessHub.Application.DTOs;
using AccessHub.Application.Interfaces;
using AccessHub.Domain.Constants;
using AccessHub.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccessHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenService tokenService,
    IPermissionService permissionService,
    IAccessHubDbContext db) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var user = await userManager.Users
            .Include(u => u.Organization)
            .FirstOrDefaultAsync(u => u.Email == request.Email, ct);

        if (user is null || !user.IsActive)
            return Unauthorized(new { message = "Invalid credentials." });

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid credentials." });

        var permissions = await permissionService.GetUserPermissionsAsync(user.Id, ct);
        var token = await tokenService.GenerateTokenAsync(user, ct);

        return Ok(new LoginResponse(
            token,
            user.Id,
            user.Email!,
            user.FullName,
            user.OrganizationId,
            user.Organization?.Name,
            user.IsSuperAdmin,
            permissions));
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            return BadRequest(new { message = "An account with this email already exists." });

        var org = new Organization
        {
            Id = Guid.NewGuid(),
            Name = request.OrganizationName,
            Slug = await GenerateUniqueSlugAsync(request.OrganizationName, ct),
            IsActive = true
        };
        db.Organizations.Add(org);

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true,
            FullName = request.FullName,
            OrganizationId = org.Id,
            IsActive = true,
            IsSuperAdmin = false
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            return BadRequest(new { message = "Registration failed.", errors = createResult.Errors.Select(e => e.Description) });

        var permissions = await db.Permissions.ToListAsync(ct);
        var adminRole = new Role
        {
            Id = Guid.NewGuid(),
            OrganizationId = org.Id,
            Name = "OrgAdmin",
            Description = "Full organization administrator"
        };
        db.OrgRoles.Add(adminRole);

        foreach (var p in permissions.Where(p => p.Code != PermissionCodes.OrganizationsManage))
            db.RolePermissions.Add(new RolePermission { RoleId = adminRole.Id, PermissionId = p.Id });

        db.OrgUserRoles.Add(new UserRole { UserId = user.Id, RoleId = adminRole.Id });

        await db.SaveChangesAsync(ct);

        var grantedPermissions = await permissionService.GetUserPermissionsAsync(user.Id, ct);
        var token = await tokenService.GenerateTokenAsync(user, ct);

        return Ok(new LoginResponse(
            token,
            user.Id,
            user.Email!,
            user.FullName,
            user.OrganizationId,
            org.Name,
            user.IsSuperAdmin,
            grantedPermissions));
    }

    private async Task<string> GenerateUniqueSlugAsync(string name, CancellationToken ct)
    {
        var baseSlug = Slugify(name);
        if (string.IsNullOrEmpty(baseSlug))
            baseSlug = "org";

        var slug = baseSlug;
        while (await db.Organizations.AnyAsync(o => o.Slug == slug, ct))
            slug = $"{baseSlug}-{Guid.NewGuid().ToString("N")[..6]}";

        return slug;
    }

    private static string Slugify(string value)
    {
        var lowered = value.Trim().ToLowerInvariant();
        var replaced = Regex.Replace(lowered, @"[^a-z0-9]+", "-");
        return replaced.Trim('-');
    }

    [HttpGet("me")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<ActionResult<LoginResponse>> Me(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirst("sub")!.Value);
        var user = await userManager.Users.Include(u => u.Organization)
            .FirstAsync(u => u.Id == userId, ct);
        var permissions = await permissionService.GetUserPermissionsAsync(user.Id, ct);

        return Ok(new LoginResponse(
            string.Empty,
            user.Id,
            user.Email!,
            user.FullName,
            user.OrganizationId,
            user.Organization?.Name,
            user.IsSuperAdmin,
            permissions));
    }
}
