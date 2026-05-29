using AccessHub.Application.DTOs;
using AccessHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccessHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController(IAccessHubDbContext db) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Domain.Constants.PermissionCodes.RolesRead)]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetAll(CancellationToken ct)
    {
        var permissions = await db.Permissions.AsNoTracking()
            .OrderBy(p => p.Code)
            .Select(p => new PermissionDto(p.Id, p.Code, p.DisplayName, p.Description))
            .ToListAsync(ct);
        return Ok(permissions);
    }
}
