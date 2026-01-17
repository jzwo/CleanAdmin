using BitzArt.Blazor.Cookies;
using Microsoft.JSInterop;

namespace NcpAdminAntBlazor.Client.Services;

public enum ThemeMode
{
    Light,
    Dark
}

public class ThemeService(ICookieService cookieService, IJSRuntime jsRuntime)
{
    public const string ThemeLinkId = "theme-link";
    private const string ThemeCookieKey = "app-theme";
    private const string ThemeCssPathTemplate = "theme/{0}.css";
    private const ThemeMode DefaultTheme = ThemeMode.Dark;

    private ThemeMode? _currentTheme;
    private bool _isInitialized;

    private ThemeMode CurrentTheme => _currentTheme ?? DefaultTheme;

    /// <summary>
    /// 初始化主题，从 Cookie 读取用户偏好
    /// </summary>
    private async Task<ThemeMode> InitializeAsync()
    {
        if (_isInitialized)
            return CurrentTheme;

        var cookie = await cookieService.GetAsync(ThemeCookieKey);
        _currentTheme = ParseThemeFromCookie(cookie?.Value);
        _isInitialized = true;

        return CurrentTheme;
    }

    /// <summary>
    /// 获取当前主题（确保已初始化）
    /// </summary>
    public async Task<ThemeMode> GetThemeAsync()
    {
        if (!_isInitialized)
            return await InitializeAsync();

        return CurrentTheme;
    }

    /// <summary>
    /// 获取是否为暗色主题（确保已初始化）
    /// </summary>
    public async Task<bool> GetIsDarkAsync()
    {
        var theme = await GetThemeAsync();
        return theme == ThemeMode.Dark;
    }

    public static string GetThemeCssPath(ThemeMode theme)
    {
        var themeName = GetThemeName(theme);
        return string.Format(ThemeCssPathTemplate, themeName);
    }

    /// <summary>
    /// 设置指定主题
    /// </summary>
    public async Task SetThemeAsync(ThemeMode theme)
    {
        if (_currentTheme == theme && _isInitialized)
            return;

        _currentTheme = theme;
        _isInitialized = true;

        await SaveThemeToCookieAsync(theme);
        await ApplyThemeToDocumentAsync(theme);
    }

    private static ThemeMode ParseThemeFromCookie(string? cookieValue)
    {
        return cookieValue?.ToLowerInvariant() switch
        {
            "dark" => ThemeMode.Dark,
            "light" => ThemeMode.Light,
            _ => DefaultTheme
        };
    }

    private async Task SaveThemeToCookieAsync(ThemeMode theme)
    {
        var themeValue = theme == ThemeMode.Dark ? "dark" : "light";
        var expiration = DateTimeOffset.UtcNow.AddYears(1);
        await cookieService.SetAsync(ThemeCookieKey, themeValue, expiration, sameSiteMode: SameSiteMode.Strict);
    }

    private async Task ApplyThemeToDocumentAsync(ThemeMode theme)
    {
        var themeName = GetThemeName(theme);

        // 更新 HTML 根元素的 class
        await jsRuntime.InvokeVoidAsync("eval",
            $"document.documentElement.className = '{themeName}'");

        // 更新主题 CSS 文件链接
        var href = string.Format(ThemeCssPathTemplate, themeName);
        await jsRuntime.InvokeVoidAsync("eval",
            $"document.getElementById('{ThemeLinkId}')?.setAttribute('href', '{href}')");
    }

    private static string GetThemeName(ThemeMode theme) =>
        theme == ThemeMode.Dark ? "dark" : "light";
}