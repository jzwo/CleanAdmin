using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using CleanAdmin.Web.Client.Helpers;

namespace CleanAdmin.Web.Client.Infrastructure.Auth;

public class JwtAuthStateProvider(IUserTokenStore tokenStore) : AuthenticationStateProvider
{
    private readonly AuthenticationState _anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));
    private AuthenticationState? _cachedState;
    private string? _lastTokenHash;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var (userToken, _) = await tokenStore.GetUserTokenAsync();

        if (userToken is null || !userToken.IsRefreshTokenValid)
        {
            _cachedState = null;
            _lastTokenHash = null;
            return _anonymous;
        }

        var currentHash = ComputeTokenHash(userToken.AccessToken);

        if (_cachedState != null && _lastTokenHash == currentHash)
            return _cachedState;

        var identity = new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(userToken.AccessToken), "jwt");
        _cachedState = new AuthenticationState(new ClaimsPrincipal(identity));
        _lastTokenHash = currentHash;

        return _cachedState;
    }

    public async Task NotifyLoginAsync(UserToken token, bool isPersistent)
    {
        await tokenStore.StoreUserTokenAsync(token, isPersistent);
        NotifyStateChanged();
    }

    public async Task NotifyLogoutAsync()
    {
        await tokenStore.ClearUserTokenAsync();
        NotifyStateChanged();
    }

    public void NotifyStateChanged()
    {
        _cachedState = null;
        _lastTokenHash = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static string ComputeTokenHash(string token)
    {
        var hash = System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }
}