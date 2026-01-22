using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace NcpAdminAntBlazor.Infrastructure.Auth;

/// <summary>
/// Cookie 认证事件处理器
/// </summary>
/// <param name="store">用户令牌存储服务</param>
public class CookieEvents(IUserTokenStore store) : CookieAuthenticationEvents
{
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var path = context.Request.Path;
        if (IsStaticResource(path))
        {
            return;
        }

        if (context.Principal?.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var token = await store.GetTokenAsync(context.Principal);
        if (token is null)
        {
            context.RejectPrincipal();
            return;
        }

        context.HttpContext.Items[AntBlazorConstants.HttpContextItems.AccessToken] = token;

        await base.ValidatePrincipal(context);
    }

    public override async Task SigningOut(CookieSigningOutContext context)
    {
        await store.ClearTokenAsync(context.HttpContext.User);
        await base.SigningOut(context);
    }

    /// <summary>
    /// API 请求不跳转
    /// 处理通过 Yarp 转发的 API 请求
    /// 我们需要让它返回 401，让前端拦截器去处理跳转。
    /// </summary>
    public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
    {
        // 判断是否是 API 请求
        if (IsApiRequest(context.Request))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }

        // 普通页面请求，执行默认跳转
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    }

    public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
    {
        if (IsApiRequest(context.Request))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 辅助方法：判断是否是静态资源
    /// </summary>
    private static bool IsStaticResource(PathString path)
    {
        // 1. Blazor 框架文件
        if (path.StartsWithSegments("/_framework") ||
            path.StartsWithSegments("/_content"))
        {
            return true;
        }

        // 2. 常见静态扩展名
        var value = path.Value ?? string.Empty;
        return value.EndsWith(".css", StringComparison.OrdinalIgnoreCase) ||
               value.EndsWith(".js", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 辅助方法：判断是否 API 请求
    /// </summary>
    private static bool IsApiRequest(HttpRequest request)
    {
        return request.Path.StartsWithSegments("/api");
    }
}