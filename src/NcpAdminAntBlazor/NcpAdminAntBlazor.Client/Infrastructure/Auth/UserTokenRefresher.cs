using NcpAdminAntBlazor.Client.ApiSdk;
using NcpAdminAntBlazor.Client.Infrastructure.Http;

namespace NcpAdminAntBlazor.Client.Infrastructure.Auth;

public interface IUserTokenRefresher
{
    Task<UserToken?> RefreshTokenAsync(string userId, string refreshToken, bool isPersistent, CancellationToken cancellationToken = default);
}

public class UserTokenRefresher(
    Lazy<ApiClient> lazyApiClient,
    IUserTokenStore userTokenStore,
    JwtAuthStateProvider authStateProvider,
    ILogger<UserTokenRefresher> logger)
    : IUserTokenRefresher
{
    private Task<UserToken?>? _currentRefreshTask;
    private readonly Lock _refreshLock = new();

    public Task<UserToken?> RefreshTokenAsync(string userId, string refreshToken, bool isPersistent,
        CancellationToken cancellationToken = default)
    {
        lock (_refreshLock)
        {
            if (_currentRefreshTask is { IsCompleted: false })
            {
                return _currentRefreshTask;
            }

            _currentRefreshTask = RefreshLogicAsync(userId, refreshToken, isPersistent, cancellationToken);
            return _currentRefreshTask;
        }
    }

    private async Task<UserToken?> RefreshLogicAsync(string userId, string refreshToken, bool isPersistent,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("🔄 Starting token refresh for User:{UserId}", userId);
            var apiClient = lazyApiClient.Value;
            var response = await apiClient.Api.Auth.RefreshToken.PostAsync(
                new()
                {
                    UserId = userId,
                    RefreshToken = refreshToken
                }, config => { config.Options.Add(AnonymousRequestOption.Instance); }, cancellationToken);

            if (response is { Success: true, Data: { } data })
            {
                var newToken = new UserToken(
                    userId,
                    data.AccessToken ?? string.Empty,
                    data.AccessTokenExpiry ?? DateTimeOffset.UtcNow,
                    data.RefreshToken ?? string.Empty,
                    data.RefreshTokenExpiry ?? DateTimeOffset.UtcNow
                );

                await userTokenStore.StoreUserTokenAsync(newToken, isPersistent, cancellationToken);

                authStateProvider.NotifyStateChanged();

                logger.LogInformation("✅ Token refreshed successfully.");
                return newToken;
            }

            logger.LogWarning("❌ Refresh failed. Server returned success=false.");
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "🔥 Exception occurred while refreshing token.");
            return null;
        }
    }
}