using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace NcpAdminBlazor.Web.Infrastructure.Auth;

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

        context.HttpContext.Items[WebConstants.HttpContextItems.AccessToken] = token;

        await base.ValidatePrincipal(context);
    }

    public override async Task SigningOut(CookieSigningOutContext context)
    {
        await store.ClearTokenAsync(context.HttpContext.User);
        await base.SigningOut(context);
    }

    //【关键】API 请求不跳转 (AJAX/Fetch 优化)
    // 默认情况下，未登录访问接口会返回 302 跳转到 /login。
    // 对于 Blazor/API 调用，这会导致 fetch 报错或重定向循环。
    // 我们需要让它返回 401，让前端拦截器去处理跳转。
    public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
    {
        // 判断是否是 API 请求 (根据路径或请求头)
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


    // 辅助方法：判断是否是静态资源
    private static bool IsStaticResource(PathString path)
    {
        // 1. Blazor 框架文件
        if (path.StartsWithSegments("/_framework") ||
            path.StartsWithSegments("/_content"))
            return true;

        // 2. 常见静态扩展名
        // 也可以检查 context.Request.Headers["Accept"] 是否包含 text/html
        var value = path.Value ?? string.Empty;
        return value.EndsWith(".css", StringComparison.OrdinalIgnoreCase) ||
               value.EndsWith(".js", StringComparison.OrdinalIgnoreCase);
    }

    // 辅助方法：判断是否 API 请求
    private static bool IsApiRequest(HttpRequest request)
    {
        return request.Path.StartsWithSegments("/api");
    }
}