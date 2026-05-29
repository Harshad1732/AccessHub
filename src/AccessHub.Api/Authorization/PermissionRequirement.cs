using Microsoft.AspNetCore.Authorization;

namespace AccessHub.Api.Authorization;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
