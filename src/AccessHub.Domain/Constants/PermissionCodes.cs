namespace AccessHub.Domain.Constants;

public static class PermissionCodes
{
    public const string OrganizationsManage = "organizations.manage";
    public const string UsersRead = "users.read";
    public const string UsersWrite = "users.write";
    public const string RolesRead = "roles.read";
    public const string RolesWrite = "roles.write";
    public const string AuditRead = "audit.read";
    public const string InvoicesRead = "invoices.read";
    public const string InvoicesWrite = "invoices.write";

    public static readonly string[] All =
    [
        OrganizationsManage,
        UsersRead,
        UsersWrite,
        RolesRead,
        RolesWrite,
        AuditRead,
        InvoicesRead,
        InvoicesWrite
    ];
}
