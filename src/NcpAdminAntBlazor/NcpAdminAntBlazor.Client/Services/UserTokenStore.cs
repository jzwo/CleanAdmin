using Bit.Butil;
using System.Text.Json;

namespace NcpAdminAntBlazor.Client.Services;

public interface IUserTokenStore
{
    Task<(UserToken? Token, bool IsPersistent)> GetUserTokenAsync(CancellationToken cancellationToken = default);
    Task StoreUserTokenAsync(UserToken userToken, bool isPersistent, CancellationToken cancellationToken = default);
    Task ClearUserTokenAsync(CancellationToken cancellationToken = default);
}

public class UserTokenStore(
    SessionStorage sessionStorage,
    LocalStorage localStorage,
    ILogger<UserTokenStore> logger) : IUserTokenStore
{
    private const string StorageKey = "user_auth_token";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private UserToken? _cachedToken;
    private bool? _cachedIsPersistent;

    public async Task<(UserToken? Token, bool IsPersistent)> GetUserTokenAsync(
        CancellationToken cancellationToken = default)
    {
        if (_cachedToken is not null)
        {
            return (_cachedToken, _cachedIsPersistent ?? false);
        }

        var localJson = await localStorage.GetItem(StorageKey);
        if (!string.IsNullOrWhiteSpace(localJson))
        {
            try
            {
                var token = JsonSerializer.Deserialize<UserToken>(localJson, JsonOptions);
                if (token?.IsValid == true)
                {
                    _cachedToken = token;
                    _cachedIsPersistent = true;
                    return (token, true);
                }
                    
                logger.LogWarning("Invalid token format in localStorage");
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Failed to deserialize token from localStorage, clearing storage");
                await ClearUserTokenAsync(cancellationToken);
                return (null, false);
            }
        }

        var sessionJson = await sessionStorage.GetItem(StorageKey);
        if (!string.IsNullOrWhiteSpace(sessionJson))
        {
            try
            {
                var token = JsonSerializer.Deserialize<UserToken>(sessionJson, JsonOptions);
                if (token?.IsValid == true)
                {
                    _cachedToken = token;
                    _cachedIsPersistent = false;
                    return (token, false);
                }
                    
                logger.LogWarning("Invalid token format in sessionStorage");
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Failed to deserialize token from sessionStorage, clearing storage");
                await ClearUserTokenAsync(cancellationToken);
            }
        }

        return (null, false);
    }

    public async Task StoreUserTokenAsync(UserToken userToken, bool isPersistent,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userToken);
        
        if (!userToken.IsValid)
        {
            logger.LogWarning("Attempted to store invalid token for user {UserId}", userToken.UserId);
            throw new ArgumentException("Cannot store invalid token", nameof(userToken));
        }

        var json = JsonSerializer.Serialize(userToken, JsonOptions);

        if (isPersistent)
        {
            await Task.WhenAll(
                localStorage.SetItem(StorageKey, json),
                sessionStorage.RemoveItem(StorageKey)
            );
        }
        else
        {
            await Task.WhenAll(
                sessionStorage.SetItem(StorageKey, json),
                localStorage.RemoveItem(StorageKey)
            );
        }

        _cachedToken = userToken;
        _cachedIsPersistent = isPersistent;
    }

    public async Task ClearUserTokenAsync(CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            localStorage.RemoveItem(StorageKey),
            sessionStorage.RemoveItem(StorageKey)
        );

        _cachedToken = null;
        _cachedIsPersistent = null;
    }
}

public record UserToken(
    string UserId,
    string AccessToken,
    DateTimeOffset AccessTokenExpiry,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiry
)
{
    private static readonly TimeSpan ExpiryBuffer = TimeSpan.FromSeconds(30);

    public DateTimeOffset AccessTokenExpiry { get; init; } = AccessTokenExpiry.ToUniversalTime();
    public DateTimeOffset RefreshTokenExpiry { get; init; } = RefreshTokenExpiry.ToUniversalTime();

    public bool IsAccessTokenValid => DateTimeOffset.UtcNow.Add(ExpiryBuffer) < AccessTokenExpiry;
    public bool IsRefreshTokenValid => DateTimeOffset.UtcNow.Add(ExpiryBuffer) < RefreshTokenExpiry;

    public bool IsValid => !string.IsNullOrWhiteSpace(UserId) 
                           && !string.IsNullOrWhiteSpace(AccessToken) 
                           && !string.IsNullOrWhiteSpace(RefreshToken);
}