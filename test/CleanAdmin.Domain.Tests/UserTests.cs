using CleanAdmin.Domain.AggregatesModel.RoleAggregate;
using CleanAdmin.Domain.AggregatesModel.UserAggregate;

namespace CleanAdmin.Domain.Tests;

public class UserTests
{
    [Fact]
    public void CreateUser_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var username = "testuser";
        var passwordHash = "hashedpassword";
        var realName = "Test User";
        var email = "test@example.com";
        var phone = "13800138000";
        var userRoles = new List<UserRole>
        {
            new(new RoleId(Guid.NewGuid()), "Admin"),
            new(new RoleId(Guid.NewGuid()), "User")
        };

        // Act
        var user = new User(username, passwordHash, realName, email, phone, userRoles, []);

        // Assert
        Assert.Equal(username, user.Username);
        Assert.Equal(passwordHash, user.PasswordHash);
        Assert.Equal(realName, user.RealName);
        Assert.Equal(email, user.Email);
        Assert.Equal(phone, user.Phone);
        Assert.Equal(2, user.UserRoles.Count);
        Assert.Empty(user.UserPermissions);
        Assert.False(user.IsDeleted);
    }

    [Fact]
    public void UpdateInfo_ShouldUpdateUserProperties()
    {
        // Arrange
        var user = CreateTestUser();
        var newUsername = "newusername";
        var newRealName = "New Name";
        var newEmail = "new@example.com";
        var newPhone = "13900139000";
        var newRoles = new List<UserRole>
        {
            new(new RoleId(Guid.NewGuid()), "SuperAdmin")
        };

        // Act
        user.UpdateInfo(newUsername, newRealName, newEmail, newPhone, newRoles, []);

        // Assert
        Assert.Equal(newUsername, user.Username);
        Assert.Equal(newRealName, user.RealName);
        Assert.Equal(newEmail, user.Email);
        Assert.Equal(newPhone, user.Phone);
        Assert.Single(user.UserRoles);
        Assert.Equal("SuperAdmin", user.UserRoles.First().RoleName);
    }

    [Fact]
    public void ChangePassword_ShouldUpdatePasswordHash()
    {
        // Arrange
        var user = CreateTestUser();
        var newPasswordHash = "newhashedpassword";

        // Act
        user.ChangePassword(newPasswordHash);

        // Assert
        Assert.Equal(newPasswordHash, user.PasswordHash);
    }

    [Fact]
    public void ChangePassword_WithSamePassword_ShouldThrowException()
    {
        // Arrange
        var user = CreateTestUser();
        var samePasswordHash = "hashedpassword";

        // Act & Assert
        var exception = Assert.Throws<KnownException>(() => user.ChangePassword(samePasswordHash));
        Assert.Equal("新密码不能与旧密码相同", exception.Message);
    }

    [Fact]
    public void SetRefreshToken_ShouldUpdateTokenAndExpiry()
    {
        // Arrange
        var user = CreateTestUser();
        var refreshToken = "refreshtoken123";
        var expiry = DateTimeOffset.UtcNow.AddDays(7);

        // Act
        user.SetRefreshToken(refreshToken, expiry);

        // Assert
        Assert.Equal(refreshToken, user.RefreshToken);
        Assert.Equal(expiry, user.RefreshExpiry);
    }

    [Fact]
    public void Delete_ShouldMarkUserAsDeleted()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        user.Delete();

        // Assert
        Assert.True(user.IsDeleted);
    }

    [Fact]
    public void Delete_AlreadyDeletedUser_ShouldThrowException()
    {
        // Arrange
        var user = CreateTestUser();
        user.Delete();

        // Act & Assert
        var exception = Assert.Throws<KnownException>(() => user.Delete());
        Assert.Equal("用户已经被删除！", exception.Message);
    }

    [Fact]
    public void UserRole_Update_ShouldUpdateRoleName()
    {
        // Arrange
        var roleId = new RoleId(Guid.NewGuid());
        var userRole = new UserRole(roleId, "OldRole");

        // Act
        userRole.Update("NewRole");

        // Assert
        Assert.Equal("NewRole", userRole.RoleName);
        Assert.Equal(roleId, userRole.RoleId);
    }

    [Fact]
    public void UserPermission_UpdateSourceRoles_ShouldUpdateSourceRoleIds()
    {
        // Arrange
        var roleId1 = new RoleId(Guid.NewGuid());
        var roleId2 = new RoleId(Guid.NewGuid());
        var roleId3 = new RoleId(Guid.NewGuid());
        var permission = new UserPermission("test.permission", new List<RoleId> { roleId1 });

        // Act
        permission.UpdateSourceRoles(new List<RoleId> { roleId2, roleId3 });

        // Assert
        Assert.Equal(2, permission.SourceRoleIds.Count);
        Assert.Contains(roleId2, permission.SourceRoleIds);
        Assert.Contains(roleId3, permission.SourceRoleIds);
        Assert.DoesNotContain(roleId1, permission.SourceRoleIds);
    }

    private static User CreateTestUser()
    {
        var userRoles = new List<UserRole>
        {
            new(new RoleId(Guid.NewGuid()), "TestRole")
        };

        return new User(
            username: "testuser",
            passwordHash: "hashedpassword",
            realName: "Test User",
            email: "test@example.com",
            phone: "13800138000",
            userRoles: userRoles,
            rolePermissionMappings: []
        );
    }
}
