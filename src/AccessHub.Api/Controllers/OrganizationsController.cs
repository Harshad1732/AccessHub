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
public class OrganizationsController(
    IAccessHubDbContext db,
    IAuditService audit,
    CurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Domain.Constants.PermissionCodes.OrganizationsManage)]
    public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetAll(CancellationToken ct)
    {
        var orgs = await db.Organizations.AsNoTracking()
            .OrderBy(o => o.Name)
            .Select(o => new OrganizationDto(o.Id, o.Name, o.Slug, o.IsActive, o.CreatedAtUtc))
            .ToListAsync(ct);
        return Ok(orgs);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrganizationDto>> GetById(Guid id, CancellationToken ct)
    {
        if (!currentUser.IsSuperAdmin && currentUser.OrganizationId != id)
            return Forbid();

        var org = await db.Organizations.AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new OrganizationDto(o.Id, o.Name, o.Slug, o.IsActive, o.CreatedAtUtc))
            .FirstOrDefaultAsync(ct);

        return org is null ? NotFound() : Ok(org);
    }

    [HttpPost]
    [Authorize(Policy = Domain.Constants.PermissionCodes.OrganizationsManage)]
    public async Task<ActionResult<OrganizationDto>> Create([FromBody] CreateOrganizationRequest request, CancellationToken ct)
    {
        if (await db.Organizations.AnyAsync(o => o.Slug == request.Slug, ct))
            return Conflict(new { message = "Slug already exists." });

        var org = new Organization
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Slug.ToLowerInvariant(),
            IsActive = true
        };

        db.Organizations.Add(org);
        await db.SaveChangesAsync(ct);

        await audit.LogAsync(currentUser.UserId, org.Id, "Created", "Organization", org.Id.ToString(),
            new { org.Name, org.Slug }, ct);

        return CreatedAtAction(nameof(GetById), new { id = org.Id },
            new OrganizationDto(org.Id, org.Name, org.Slug, org.IsActive, org.CreatedAtUtc));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Domain.Constants.PermissionCodes.OrganizationsManage)]
    public async Task<ActionResult<OrganizationDto>> Update(Guid id, [FromBody] UpdateOrganizationRequest request, CancellationToken ct)
    {
        var org = await db.Organizations.FindAsync([id], ct);
        if (org is null) return NotFound();

        org.Name = request.Name;
        org.Slug = request.Slug.ToLowerInvariant();
        org.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);

        await audit.LogAsync(currentUser.UserId, org.Id, "Updated", "Organization", org.Id.ToString(),
            new { org.Name, org.Slug, org.IsActive }, ct);

        return Ok(new OrganizationDto(org.Id, org.Name, org.Slug, org.IsActive, org.CreatedAtUtc));
    }
}
