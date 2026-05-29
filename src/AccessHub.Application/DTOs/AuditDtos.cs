namespace AccessHub.Application.DTOs;

public record AuditEventDto(
    Guid Id,
    Guid? OrganizationId,
    Guid ActorUserId,
    string Action,
    string EntityType,
    string? EntityId,
    string? PayloadJson,
    DateTime CreatedAtUtc);
