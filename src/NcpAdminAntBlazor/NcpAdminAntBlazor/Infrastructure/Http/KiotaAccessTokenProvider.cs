using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Kiota.Abstractions.Authentication;
using NcpAdminAntBlazor.Infrastructure.Auth;

namespace NcpAdminAntBlazor.Infrastructure.Http;

/// <summary>
/// Kiota Access Token 提供器
/// 从 UserTokenStore 获取已刷新的 Access Token
/// </summary>
/// <param name="authProvider">认证状态提供程序</param>
/// <param name="userTokenStore">用户令牌存储服务</param>
/// <param name="logger">日志记录器</param>
public class KiotaAccessTokenProvider(
    AuthenticationStateProvider authProvider,
    IUserTokenStore userTokenStore,
    ILogger<KiotaAccessTokenProvider> logger)
    : IAccessTokenProvider
{
    public AllowedHostsValidator AllowedHostsValidator { get; } = new();

    public async Task<string> GetAuthorizationTokenAsync(
        Uri uri,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        var authState = await authProvider.GetAuthenticationStateAsync();
        var token = await userTokenStore.GetTokenAsync(authState.User);

        if (!string.IsNullOrEmpty(token))
        {
            return token;
        }

        logger.LogDebug("No valid access token available for Kiota request to {Uri}", uri);
        return string.Empty;
    }
}
