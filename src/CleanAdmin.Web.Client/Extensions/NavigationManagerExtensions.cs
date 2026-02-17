using Microsoft.AspNetCore.Components;
using CleanAdmin.Web.Client.Pages.Auth;

namespace CleanAdmin.Web.Client.Extensions;

/// <summary>
/// NavigationManager 扩展方法
/// </summary>
public static class NavigationManagerExtensions
{
    /// <summary>
    /// 重定向到登录页，并自动附加当前页面作为 returnUrl
    /// </summary>
    /// <param name="navigationManager">导航管理器</param>
    /// <param name="forceLoad">是否强制刷新页面，默认 false</param>
    public static void NavigateToLogin(this NavigationManager navigationManager, bool forceLoad = false)
    {
        var returnUrl = Uri.EscapeDataString(navigationManager.Uri);
        var loginUrl = $"{Login.PageUri}?returnUrl={returnUrl}";
        navigationManager.NavigateTo(loginUrl, forceLoad);
    }
}

