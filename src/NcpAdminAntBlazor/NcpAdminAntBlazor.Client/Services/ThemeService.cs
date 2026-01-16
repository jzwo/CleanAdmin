using BitzArt.Blazor.Cookies;
using Microsoft.JSInterop;

namespace NcpAdminAntBlazor.Client.Services;

public class ThemeService(ICookieService cookieService, IJSRuntime jsRuntime)
{
    private const string ThemeCookieKey = "app-theme";
    private bool? _isDark;

    public event Action? OnThemeChanged;

    public bool IsDark => _isDark ?? true; // 默认暗色主题

    public async Task InitializeAsync()
    {
        try
        {
            var cookie = await cookieService.GetAsync(ThemeCookieKey);
            var storedTheme = cookie?.Value;
            _isDark = storedTheme switch
            {
                "dark" => true,
                "light" => false,
                _ => true // 默认暗色
            };
            await UpdateHtmlClassAsync();
        }
        catch
        {
            _isDark = true;
        }
    }

    public async Task ToggleThemeAsync()
    {
        _isDark = !_isDark;
        await SaveThemeAsync();
        await UpdateHtmlClassAsync();
        OnThemeChanged?.Invoke();
    }

    public async Task SetThemeAsync(bool isDark)
    {
        if (_isDark == isDark) return;
        
        _isDark = isDark;
        await SaveThemeAsync();
        await UpdateHtmlClassAsync();
        OnThemeChanged?.Invoke();
    }

    private async Task SaveThemeAsync()
    {
        try
        {
            var themeValue = _isDark == true ? "dark" : "light";
            // Cookie 设置为 365 天过期
            var expirationDate = DateTimeOffset.UtcNow.AddDays(365);
            await cookieService.SetAsync(ThemeCookieKey, themeValue, expirationDate);
        }
        catch
        {
            // 忽略存储错误
        }
    }

    private async Task UpdateHtmlClassAsync()
    {
        try
        {
            var themeClass = _isDark == true ? "dark" : "light";
            await jsRuntime.InvokeVoidAsync("eval", $"document.documentElement.className = '{themeClass}'");
        }
        catch
        {
            // 忽略更新错误
        }
    }
}
