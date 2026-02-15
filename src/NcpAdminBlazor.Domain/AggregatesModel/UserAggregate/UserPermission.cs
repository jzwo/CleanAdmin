using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;

public partial record UserPermissionId : IGuidStronglyTypedId;

/// <summary>
/// 用户权限子实体
/// </summary>
public class UserPermission : Entity<UserPermissionId>
{
    protected UserPermission()
    {
    }

    public UserPermission(string permissionCode, ICollection<RoleId> sourceRoleIds)
    {
        PermissionCode = permissionCode;
        SourceRoleIds = sourceRoleIds;
    }

    public string PermissionCode { get; private set; } = string.Empty;
    public ICollection<RoleId> SourceRoleIds { get; private set; } = [];

    public void UpdateSourceRoles(ICollection<RoleId> sourceRoleIds)
    {
        SourceRoleIds = sourceRoleIds;
    }
}


