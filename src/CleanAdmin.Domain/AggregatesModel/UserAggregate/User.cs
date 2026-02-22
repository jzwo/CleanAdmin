using CleanAdmin.Domain.Common;
using CleanAdmin.Domain.AggregatesModel.RoleAggregate;
using CleanAdmin.Domain.DomainEvents;

namespace CleanAdmin.Domain.AggregatesModel.UserAggregate;

public partial record UserId : IGuidStronglyTypedId;

/// <summary>
/// 用户聚合根
/// </summary>
public class User : Entity<UserId>, IAggregateRoot, ISoftDeletable
{
    protected User()
    {
    }

    public string Username { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string RealName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public ICollection<UserRole> UserRoles { get; private set; } = [];
    public ICollection<UserPermission> UserPermissions { get; private set; } = [];
    public string RefreshToken { get; private set; } = string.Empty;
    public DateTimeOffset RefreshExpiry { get; private set; } = DateTimeOffset.MinValue;
    public DateTimeOffset CreatedAt { get; init; }
    public Deleted IsDeleted { get; private set; } = false;
    public DeletedTime DeletedAt { get; private set; } = new(DateTimeOffset.MinValue);

    public User(
        string username,
        string passwordHash,
        string realName,
        string email,
        string phone,
        ICollection<UserRole> userRoles,
        IEnumerable<(RoleId RoleId, IEnumerable<string> PermissionCodes)> rolePermissionMappings)
    {
        CreatedAt = DateTimeOffset.UtcNow;
        Username = username;
        RealName = realName;
        Email = email;
        Phone = phone;
        PasswordHash = passwordHash;
        UserRoles = userRoles;
        UserPermissions = BuildPermissions(rolePermissionMappings);
        AddDomainEvent(new UserCreatedDomainEvent(this));
    }

    public void UpdateInfo(string username, string realName, string email, string phone,
        ICollection<UserRole> userRoles,
        IEnumerable<(RoleId RoleId, IEnumerable<string> PermissionCodes)> rolePermissionMappings)
    {
        Username = username;
        RealName = realName;
        Email = email;
        Phone = phone;
        UserRoles = userRoles;
        UserPermissions = BuildPermissions(rolePermissionMappings);
        AddDomainEvent(new UserInfoUpdatedDomainEvent(this));
    }

    private static List<UserPermission> BuildPermissions(
        IEnumerable<(RoleId RoleId, IEnumerable<string> PermissionCodes)> rolePermissionMappings)
    {
        var permissionGroups = rolePermissionMappings
            .SelectMany(role => role.PermissionCodes.Select(code => new { role.RoleId, PermissionCode = code }))
            .Where(x => !string.IsNullOrWhiteSpace(x.PermissionCode))
            .GroupBy(x => x.PermissionCode.Trim(), StringComparer.Ordinal)
            .Select(g => new UserPermission(
                g.Key,
                g.Select(x => x.RoleId).Distinct().ToList()))
            .ToList();

        return permissionGroups;
    }

    public void UpdateBasicInfo(string realName, string email, string phone)
    {
        RealName = realName;
        Email = email;
        Phone = phone;
        AddDomainEvent(new UserInfoUpdatedDomainEvent(this));
    }

    public void UpdateRolePermissions(RoleId roleId, ICollection<string> permissionCodes)
    {
        var normalizedPermissionCodes = permissionCodes
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Select(code => code.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToHashSet(StringComparer.Ordinal);

        var currentPermissions = UserPermissions.ToList();

        foreach (var userPermission in currentPermissions)
        {
            if (!userPermission.SourceRoleIds.Contains(roleId))
            {
                continue;
            }

            if (normalizedPermissionCodes.Contains(userPermission.PermissionCode))
            {
                continue;
            }

            var updatedSourceRoleIds = userPermission.SourceRoleIds
                .Where(id => id != roleId)
                .Distinct()
                .ToList();

            if (updatedSourceRoleIds.Count == 0)
            {
                UserPermissions.Remove(userPermission);
                continue;
            }

            userPermission.UpdateSourceRoles(updatedSourceRoleIds);
        }

        foreach (var permissionCode in normalizedPermissionCodes)
        {
            var existingUserPermission = UserPermissions
                .FirstOrDefault(permission => permission.PermissionCode == permissionCode);

            if (existingUserPermission is null)
            {
                UserPermissions.Add(new UserPermission(permissionCode, [roleId]));
                continue;
            }

            if (existingUserPermission.SourceRoleIds.Contains(roleId))
            {
                continue;
            }

            var updatedSourceRoleIds = existingUserPermission.SourceRoleIds
                .Append(roleId)
                .Distinct()
                .ToList();
            existingUserPermission.UpdateSourceRoles(updatedSourceRoleIds);
        }
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (PasswordHash == newPasswordHash)
            throw new KnownException("新密码不能与旧密码相同");
        PasswordHash = newPasswordHash;
        AddDomainEvent(new UserPasswordChangedDomainEvent(this));
    }

    public void Login()
    {
        AddDomainEvent(new UserLoginDomainEvent(this));
    }

    public void SetRefreshToken(string refreshToken, DateTimeOffset refreshExpiry)
    {
        RefreshToken = refreshToken;
        RefreshExpiry = refreshExpiry;
    }

    public void Delete()
    {
        if (IsDeleted) throw new KnownException("用户已经被删除！");
        IsDeleted = true;
        AddDomainEvent(new UserDeletedDomainEvent(this));
    }
}
