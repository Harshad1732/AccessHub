namespace AccessHub.Application.DTOs;

public record OrganizationDto(Guid Id, string Name, string Slug, bool IsActive, DateTime CreatedAtUtc);

public record CreateOrganizationRequest(string Name, string Slug);

public record UpdateOrganizationRequest(string Name, string Slug, bool IsActive);
