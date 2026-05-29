namespace AccessHub.Domain.Entities;

public class Organization
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<ApplicationUser> Users { get; set; } = [];
    public ICollection<Role> Roles { get; set; } = [];
    public ICollection<AuditEvent> AuditEvents { get; set; } = [];
    public ICollection<Invoice> Invoices { get; set; } = [];
}
