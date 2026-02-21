using System.Security.Claims;
using CleanAdmin.Web.Client.ApiSdk;
using Microsoft.AspNetCore.Components.Authorization;

namespace CleanAdmin.Web.Client.Services;

public interface IUserPermissionService
{
    Task<bool> HasPermissionAsync(string permissionCode, CancellationToken cancellationToken = default);
}

public sealed class UserPermissionService(
    ApiClient apiClient,
    AuthenticationStateProvider authenticationStateProvider) : IUserPermissionService
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private HashSet<string> _permissionCodes = new(StringComparer.OrdinalIgnoreCase);
    private bool _isLoaded;
    private string? _userId;

    public async Task<bool> HasPermissionAsync(string permissionCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(permissionCode))
        {
            return false;
        }

        await EnsureLoadedAsync(cancellationToken);
        return _permissionCodes.Contains(permissionCode);
    }

    private async Task EnsureLoadedAsync(CancellationToken cancellationToken)
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            ResetCache();
            return;
        }

        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (_isLoaded && string.Equals(_userId, currentUserId, StringComparison.Ordinal))
        {
            return;
        }

        await _lock.WaitAsync(cancellationToken);
        try
        {
            authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            user = authState.User;
            currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (user.Identity?.IsAuthenticated != true)
            {
                ResetCache();
                return;
            }

            if (_isLoaded && string.Equals(_userId, currentUserId, StringComparison.Ordinal))
            {
                return;
            }

            var response = await apiClient.Api.User.Permissions.Current.GetAsync(cancellationToken: cancellationToken);
            var codes = response is { Success: true } ? response.Data ?? [] : [];

            _permissionCodes = codes
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            _userId = currentUserId;
            _isLoaded = true;
        }
        finally
        {
            _lock.Release();
        }
    }

    private void ResetCache()
    {
        _permissionCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        _isLoaded = false;
        _userId = null;
    }
}
