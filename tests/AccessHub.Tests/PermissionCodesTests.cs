using AccessHub.Domain.Constants;
using FluentAssertions;

namespace AccessHub.Tests;

public class PermissionCodesTests
{
    [Fact]
    public void All_ContainsExpectedPermissions()
    {
        PermissionCodes.All.Should().Contain(PermissionCodes.InvoicesRead);
        PermissionCodes.All.Should().Contain(PermissionCodes.OrganizationsManage);
        PermissionCodes.All.Should().HaveCount(8);
    }

    [Fact]
    public void PermissionCodes_AreUnique()
    {
        PermissionCodes.All.Should().OnlyHaveUniqueItems();
    }
}
