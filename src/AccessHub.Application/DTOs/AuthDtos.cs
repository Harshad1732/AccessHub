namespace AccessHub.Application.DTOs;

public record LoginRequest(string Email, string Password);

public record RegisterRequest(
    string OrganizationName,
    string FullName,
    string Email,
    string Password);

public record LoginResponse(
    string Token,
    Guid UserId,
    string Email,
    string FullName,
    Guid? OrganizationId,
    string? OrganizationName,
    bool IsSuperAdmin,
    IReadOnlyList<string> Permissions);

public record RegisterUserRequest(
    string Email,
    string Password,
    string FullName,
    Guid OrganizationId,
    IReadOnlyList<Guid>? RoleIds = null);
