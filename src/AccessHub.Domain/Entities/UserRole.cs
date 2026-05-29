using System.ComponentModel.DataAnnotations.Schema;

namespace AccessHub.Domain.Entities;

[Table("OrgUserRoles")]
public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime AssignedAtUtc { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
