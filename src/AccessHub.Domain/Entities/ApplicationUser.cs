using Microsoft.AspNetCore.Identity;

namespace AccessHub.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public Guid? OrganizationId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsSuperAdmin { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Organization? Organization { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = [];
}
