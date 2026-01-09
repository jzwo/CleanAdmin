using Microsoft.AspNetCore.Authentication.Cookies;
using NcpAdminBlazor.Web.Infrastructure.Auth;

namespace NcpAdminBlazor.Web.Extensions;

/// <summary>
/// Cookie 认证服务扩展方法
/// 配置基于 Cookie 的认证，并支持自动刷新 Access Token
/// 基于 ASP.NET Core OpenID Connect cookie refresh 模式实现
/// </summary>
public static class CookieAuthenticationExtensions
{
    /// <summary>
    /// 添加 Cookie 认证服务，支持自动刷新 Token
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合（用于链式调用）</returns>
    public static IServiceCollection AddCookieAuthenticationWithRefresh(this IServiceCollection services)
    {
        services.AddScoped<IUserTokenStore, UserTokenStore>();
        services.AddScoped<IUserTokenRefresher, UserTokenRefresher>();

        // 注册 Cookie 刷新服务
        services.AddTransient<CookieEvents>();

        // 添加 Cookie 认证方案
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.EventsType = typeof(CookieEvents);

                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict;

                // 路径配置
                options.LoginPath = "/account/login"; // 未登录时跳转地址
                // options.AccessDeniedPath = "/access-denied"; // 403 禁止访问地址
            });
        return services;
    }
}