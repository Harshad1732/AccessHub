using System.Security.Claims;

namespace AccessHub.Api.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor)
{
    public Guid UserId =>
        Guid.Parse(httpContextAccessor.HttpContext?.User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    public Guid? OrganizationId
    {
        get
        {
            var orgClaim = httpContextAccessor.HttpContext?.User.FindFirstValue("org_id");
            return orgClaim is null ? null : Guid.Parse(orgClaim);
        }
    }

    public bool IsSuperAdmin =>
        httpContextAccessor.HttpContext?.User.HasClaim("is_super_admin", "true") == true;

    public IReadOnlyList<string> Permissions =>
        httpContextAccessor.HttpContext?.User.FindAll("permission").Select(c => c.Value).ToList()
        ?? [];
}
