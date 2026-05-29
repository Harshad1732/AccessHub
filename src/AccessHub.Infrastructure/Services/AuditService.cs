using System.Text.Json;
using AccessHub.Application.Interfaces;
using AccessHub.Domain.Entities;

namespace AccessHub.Infrastructure.Services;

public class AuditService(IAccessHubDbContext db) : IAuditService
{
    public async Task LogAsync(
        Guid actorUserId,
        Guid? organizationId,
        string action,
        string entityType,
        string? entityId,
        object? payload = null,
        CancellationToken cancellationToken = default)
    {
        db.AuditEvents.Add(new AuditEvent
        {
            Id = Guid.NewGuid(),
            ActorUserId = actorUserId,
            OrganizationId = organizationId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            PayloadJson = payload is null ? null : JsonSerializer.Serialize(payload),
            CreatedAtUtc = DateTime.UtcNow
        });

        await db.SaveChangesAsync(cancellationToken);
    }
}
