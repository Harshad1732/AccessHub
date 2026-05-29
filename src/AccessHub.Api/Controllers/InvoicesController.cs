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
public class InvoicesController(
    IAccessHubDbContext db,
    CurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Domain.Constants.PermissionCodes.InvoicesRead)]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetAll(CancellationToken ct)
    {
        var orgId = currentUser.OrganizationId;
        if (orgId is null && !currentUser.IsSuperAdmin)
            return Forbid();

        var query = db.Invoices.AsNoTracking();
        if (orgId.HasValue)
            query = query.Where(i => i.OrganizationId == orgId.Value);

        var invoices = await query
            .OrderByDescending(i => i.CreatedAtUtc)
            .Select(i => new InvoiceDto(i.Id, i.Number, i.CustomerName, i.Amount, i.CreatedAtUtc))
            .ToListAsync(ct);

        return Ok(invoices);
    }

    [HttpPost]
    [Authorize(Policy = Domain.Constants.PermissionCodes.InvoicesWrite)]
    public async Task<ActionResult<InvoiceDto>> Create([FromBody] CreateInvoiceRequest request, CancellationToken ct)
    {
        var orgId = currentUser.OrganizationId;
        if (orgId is null)
            return BadRequest(new { message = "Organization context required." });

        if (await db.Invoices.AnyAsync(i => i.OrganizationId == orgId && i.Number == request.Number, ct))
            return Conflict(new { message = "Invoice number already exists." });

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId.Value,
            Number = request.Number,
            CustomerName = request.CustomerName,
            Amount = request.Amount
        };

        db.Invoices.Add(invoice);
        await db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetAll), null,
            new InvoiceDto(invoice.Id, invoice.Number, invoice.CustomerName, invoice.Amount, invoice.CreatedAtUtc));
    }
}
