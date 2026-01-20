using System.Globalization;
using Bit.Butil;
using Microsoft.AspNetCore.Components;

namespace NcpAdminAntBlazor.Client.Services;

public interface ICultureService
{
    public Task SetCultureAsync(string culture);
    public Task<CultureInfo> GetStoredOrDefaultCultureInfoAsync();
}

public class CultureService(
    ICultureOptions cultureOptions,
    LocalStorage localStorage,
    NavigationManager navigationManager)
    : ICultureService
{
    public async Task SetCultureAsync(string culture)
    {
        await localStorage.SetItem(cultureOptions.LocalStorageKey, culture);
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
        var storedCultureName = await localStorage.GetItem(cultureOptions.LocalStorageKey);
        return !string.IsNullOrEmpty(storedCultureName)
            ? new CultureInfo(storedCultureName)
            : new CultureInfo(cultureOptions.DefaultCulture);
    }
}