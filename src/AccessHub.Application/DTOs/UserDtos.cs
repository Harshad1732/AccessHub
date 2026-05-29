namespace AccessHub.Application.DTOs;

public record UserDto(
    Guid Id,
    string Email,
    string FullName,
    Guid? OrganizationId,
    bool IsActive,
    bool IsSuperAdmin,
    IReadOnlyList<Guid> RoleIds);

public record CreateUserRequest(
    string Email,
    string Password,
    string FullName,
    Guid OrganizationId,
    IReadOnlyList<Guid>? RoleIds);

public record UpdateUserRequest(string FullName, bool IsActive, IReadOnlyList<Guid>? RoleIds);
