using AccessHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessHub.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Number).HasMaxLength(50).IsRequired();
        builder.Property(i => i.CustomerName).HasMaxLength(200).IsRequired();
        builder.Property(i => i.Amount).HasPrecision(18, 2);
        builder.HasIndex(i => new { i.OrganizationId, i.Number }).IsUnique();
        builder.HasOne(i => i.Organization).WithMany(o => o.Invoices).HasForeignKey(i => i.OrganizationId);
    }
}
