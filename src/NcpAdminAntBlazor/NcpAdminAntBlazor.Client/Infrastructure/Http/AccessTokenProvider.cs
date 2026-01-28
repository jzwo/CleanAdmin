using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.Kiota.Abstractions.Authentication;
using NcpAdminAntBlazor.Client.Exceptions;
using NcpAdminAntBlazor.Client.Services;

namespace NcpAdminAntBlazor.Client.Infrastructure.Http;

public sealed class AccessTokenProvider(
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
            var token = await userTokenRefresher.GetRefreshedAccessTokenAsync(cancellationToken);
            return token;
        }
        catch (UserRequiresLoginException ex)
        {
            await messageService.ErrorAsync(ex.Message);
            navigationManager.NavigateToLogin();
            throw;
        }
    }
}