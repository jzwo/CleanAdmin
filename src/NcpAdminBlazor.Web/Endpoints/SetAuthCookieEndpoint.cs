using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NcpAdminBlazor.Client.Shared;
using NcpAdminBlazor.Web.Infrastructure.Auth;

namespace NcpAdminBlazor.Web.Endpoints;

/// <summary>
/// 认证端点扩展
/// </summary>
public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("bff-api/auth")
            .WithTags("Authentication");

        group.MapPost("/login", LoginAsync)
            .WithName("Login")
            .AllowAnonymous();

        group.MapPost("/logout", LogoutAsync)
            .WithName("Logout");

        return endpoints;
    }

    /// <summary>
    /// 登录端点 - 设置认证 Cookie
    /// </summary>
    private static async Task<IResult> LoginAsync(
        [FromBody] SetAuthCookieRequest request,
        IUserTokenStore userTokenStore,
        HttpContext httpContext)
    {
        if (string.IsNullOrWhiteSpace(request.AccessToken))
        {
            return Results.BadRequest(new { error = "AccessToken is required" });
        }

        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            return Results.BadRequest(new { error = "UserId is required" });
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, request.UserId)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // 配置认证属性，根据 RememberMe 设置持久化 Cookie
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = request.RememberMe,
            // 如果选择记住我，设置过期时间为 RefreshToken 的过期时间
            // 如果不选择记住我，Cookie 将是会话 Cookie（浏览器关闭时删除）
            ExpiresUtc = request.RememberMe ? request.RefreshTokenExpiry : null
        };

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            principal, 
            authProperties);

        // 存储 Token 时传递 RememberMe 参数，用于控制缓存过期策略
        await userTokenStore.StoreTokenAsync(new UserTokenData(
            request.UserId,
            request.AccessToken,
            request.RefreshToken,
            request.AccessTokenExpiry,
            request.RefreshTokenExpiry,
            request.RememberMe));

        return Results.Ok(new { message = "Authentication cookie set successfully" });
    }

    /// <summary>
    /// 登出端点 - 清除认证 Cookie
    /// </summary>
    private static async Task<IResult> LogoutAsync(
        HttpContext httpContext,
        IUserTokenStore userTokenStore)
    {
        await userTokenStore.ClearTokenAsync(httpContext.User);
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Results.Ok(new { message = "Logout successful" });
    }
}