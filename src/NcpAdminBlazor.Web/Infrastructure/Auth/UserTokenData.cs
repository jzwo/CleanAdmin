namespace NcpAdminBlazor.Web.Infrastructure.Auth;

public record UserTokenData(
    string UserId,
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAt,
    DateTimeOffset RefreshTokenExpiresAt,
    bool RememberMe = true); // 默认为 true 保持向后兼容
