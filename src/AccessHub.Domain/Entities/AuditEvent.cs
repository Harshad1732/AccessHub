namespace AccessHub.Domain.Entities;

public class AuditEvent
{
    public Guid Id { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid ActorUserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? PayloadJson { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Organization? Organization { get; set; }
}
