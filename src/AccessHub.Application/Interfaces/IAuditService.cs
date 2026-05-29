namespace AccessHub.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(
        Guid actorUserId,
        Guid? organizationId,
        string action,
        string entityType,
        string? entityId,
        object? payload = null,
        CancellationToken cancellationToken = default);
}
