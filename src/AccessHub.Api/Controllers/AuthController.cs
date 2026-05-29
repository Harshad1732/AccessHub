using AccessHub.Application.DTOs;
using AccessHub.Application.Interfaces;
using AccessHub.Domain.Entities;
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
    IPermissionService permissionService) : ControllerBase
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
