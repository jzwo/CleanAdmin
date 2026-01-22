namespace NcpAdminAntBlazor.Infrastructure.Auth;

/// <summary>
/// 用户令牌数据记录
/// </summary>
/// <param name="UserId">用户标识</param>
/// <param name="AccessToken">访问令牌</param>
/// <param name="RefreshToken">刷新令牌</param>
/// <param name="AccessTokenExpiresAt">访问令牌过期时间</param>
/// <param name="RefreshTokenExpiresAt">刷新令牌过期时间</param>
/// <param name="RememberMe">是否记住登录状态（默认为 true 保持向后兼容）</param>
public record UserTokenData(
    string UserId,
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAt,
    DateTimeOffset RefreshTokenExpiresAt,
    bool RememberMe = true);
