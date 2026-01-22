using System.Globalization;
using Bit.Butil;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace NcpAdminAntBlazor.Client.Services;

public class CultureOptions
{
    public string LocalStorageKey { get; set; } = "BlazorCulture";
    public string DefaultCulture { get; set; } = "zh-CN";
    public string[] SupportedCultures { get; set; } = ["zh-CN", "en-US"];
}

public interface ICultureService
{
    public Task SetCultureAsync(string culture);
    public Task<CultureInfo> GetStoredOrDefaultCultureInfoAsync();
}

public class CultureService(
    IOptions<CultureOptions> cultureOptions,
    LocalStorage localStorage,
    NavigationManager navigationManager)
    : ICultureService
{
    public async Task SetCultureAsync(string culture)
    {
        await localStorage.SetItem(cultureOptions.Value.LocalStorageKey, culture);
        var uri = new Uri(navigationManager.Uri)
            .GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
        var cultureEscaped = Uri.EscapeDataString(culture);
        var uriEscaped = Uri.EscapeDataString(uri);

        navigationManager.NavigateTo(
            $"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}",
            forceLoad: true);
    }

    public async Task<CultureInfo> GetStoredOrDefaultCultureInfoAsync()
    {
        var storedCultureName = await localStorage.GetItem(cultureOptions.Value.LocalStorageKey);
        return !string.IsNullOrEmpty(storedCultureName)
            ? new CultureInfo(storedCultureName)
            : new CultureInfo(cultureOptions.Value.DefaultCulture);
    }
}