using AccessHub.Api.Services;
using AccessHub.Application.DTOs;
using AccessHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccessHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = Domain.Constants.PermissionCodes.AuditRead)]
public class AuditController(IAccessHubDbContext db, CurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditEventDto>>> GetAll(
        [FromQuery] Guid? organizationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var orgId = currentUser.IsSuperAdmin ? organizationId : currentUser.OrganizationId;
        if (orgId is null && !currentUser.IsSuperAdmin)
            return Forbid();

        var query = db.AuditEvents.AsNoTracking();
        if (orgId.HasValue)
            query = query.Where(a => a.OrganizationId == orgId);

        var events = await query
            .OrderByDescending(a => a.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AuditEventDto(a.Id, a.OrganizationId, a.ActorUserId, a.Action, a.EntityType, a.EntityId, a.PayloadJson, a.CreatedAtUtc))
            .ToListAsync(ct);

        return Ok(events);
    }
}
