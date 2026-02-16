using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NcpAdminBlazor.ApiService.Application.Queries.Users;
using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;

namespace NcpAdminBlazor.ApiService.Auth.ApiKey;

internal sealed class ApikeyAuth(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IConfiguration config,
    IMediator mediator)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    internal const string SchemeName = "ApiKey";
    internal const string HeaderName = "x-api-key";

    private readonly string _apiKey = config["Auth:ApiKey"] ??
                                      throw new InvalidOperationException("Api key not set in appsettings.json");

    private readonly string _apiKeyUsername = config["Auth:ApiKeyUsername"] ?? SystemDefaultSuperAdmin.Username;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var isPublicEndpoint = IsPublicEndpoint();
        var extractedApiKey = GetApiKeyFromRequest();

        if (StringValues.IsNullOrEmpty(extractedApiKey))
        {
            return isPublicEndpoint
                ? AuthenticateResult.NoResult()
                : AuthenticateResult.Fail("Missing API credentials.");
        }

        if (!IsApiKeyValid(extractedApiKey.ToString()))
        {
            return isPublicEndpoint
                ? AuthenticateResult.NoResult()
                : AuthenticateResult.Fail("Invalid API credentials.");
        }

        var user = await InitLoginUserAsync(Context.RequestAborted);
        if (user is null)
        {
            return AuthenticateResult.Fail("ApiKey user is not configured or does not exist.");
        }

        return AuthenticateResult.Success(CreateTicket(user));
    }

    private StringValues GetApiKeyFromRequest()
    {
        Request.Headers.TryGetValue(HeaderName, out var extractedApiKey);
        if (!StringValues.IsNullOrEmpty(extractedApiKey))
        {
            return extractedApiKey;
        }

        Request.Query.TryGetValue(HeaderName, out extractedApiKey);
        return extractedApiKey;
    }

    private bool IsApiKeyValid(string providedApiKey)
    {
        var configuredBytes = System.Text.Encoding.UTF8.GetBytes(_apiKey);
        var providedBytes = System.Text.Encoding.UTF8.GetBytes(providedApiKey);
        return CryptographicOperations.FixedTimeEquals(configuredBytes, providedBytes);
    }

    private async Task<LoginUser?> InitLoginUserAsync(CancellationToken cancellationToken)
    {
        var userId = await mediator.Send(new GetUserIdByNameQuery(_apiKeyUsername), cancellationToken);
        return userId is null ? null : new LoginUser(userId.ToString(), _apiKeyUsername);
    }

    private AuthenticationTicket CreateTicket(LoginUser user)
    {
        var claims = CreateAuthenticatedUserClaims(user);

        var identity = new ClaimsIdentity(claims, authenticationType: Scheme.Name);
        var principal = new GenericPrincipal(identity, roles: null);
        return new AuthenticationTicket(principal, Scheme.Name);
    }

    private static Claim[] CreateAuthenticatedUserClaims(LoginUser user) =>
    [
        new("ClientID", "Default"),
        new(ClaimTypes.NameIdentifier, user.UserId),
        new(ClaimTypes.Name, user.UserName),
        new(ClaimTypes.AuthenticationMethod, SchemeName)
    ];

    private bool IsPublicEndpoint()
        => Context.GetEndpoint()?.Metadata.OfType<AllowAnonymousAttribute>().Any() is null or true;
}

/// <summary>
/// 表示已登录用户的不可变数据
/// </summary>
/// <param name="UserId">用户唯一标识</param>
/// <param name="UserName">用户名</param>
internal sealed record LoginUser(string UserId, string UserName);
