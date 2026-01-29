using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.Kiota.Abstractions.Authentication;
using NcpAdminAntBlazor.Client.Exceptions;
using NcpAdminAntBlazor.Client.Extensions;
using NcpAdminAntBlazor.Client.Infrastructure.Auth;

namespace NcpAdminAntBlazor.Client.Infrastructure.Http;

public sealed class AccessTokenProvider(
    IUserTokenStore userTokenStore,
    IUserTokenRefresher userTokenRefresher,
    NavigationManager navigationManager,
    MessageService messageService)
    : IAccessTokenProvider
{
    public AllowedHostsValidator AllowedHostsValidator { get; } = new();

    public async Task<string> GetAuthorizationTokenAsync(
        Uri uri,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (userToken, isPersistent) = await userTokenStore.GetUserTokenAsync(cancellationToken);

            if (userToken is null)
            {
                throw new UserRequiresLoginException("No local token found.");
            }

            if (userToken.IsAccessTokenValid)
            {
                return userToken.AccessToken;
            }

            if (!userToken.IsRefreshTokenValid)
            {
                await userTokenStore.ClearUserTokenAsync(cancellationToken);
                throw new UserRequiresLoginException("Refresh token expired.");
            }

            var refreshedToken = await userTokenRefresher.RefreshTokenAsync(
                userToken.UserId,
                userToken.RefreshToken,
                isPersistent,
                cancellationToken);

            return refreshedToken?.AccessToken is null
                ? throw new UserRequiresLoginException("Token refresh failed.")
                : refreshedToken.AccessToken;
        }
        catch (UserRequiresLoginException ex)
        {
            await messageService.ErrorAsync(ex.Message);
            navigationManager.NavigateToLogin();
            throw;
        }
    }
}