using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;

public partial record UserRoleId : IGuidStronglyTypedId;

/// <summary>
/// 用户角色子实体
/// </summary>
public class UserRole : Entity<UserRoleId>
{
    protected UserRole()
    {
    }

    public UserRole(RoleId roleId, string roleName)
    {
        RoleId = roleId;
        RoleName = roleName;
    }

    public RoleId RoleId { get; private set; } = null!;
    public string RoleName { get; private set; } = string.Empty;

    public void Update(string roleName)
    {
        RoleName = roleName;
    }
}