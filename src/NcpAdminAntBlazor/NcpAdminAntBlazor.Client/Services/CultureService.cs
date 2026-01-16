using System.Globalization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace NcpAdminAntBlazor.Client.Services;

public interface ICultureService
{
    public Task SetCultureAsync(string culture);
    public Task<CultureInfo> GetStoredOrDefalutCultureInfoAsync();
}

public class CultureService(
    ICultureOptions cultureOptions,
    ILocalStorageService localStorageService,
    NavigationManager navigationManager)
    : ICultureService
{
    public async Task SetCultureAsync(string culture)
    {
        await localStorageService.SetItemAsync(cultureOptions.LocalStorageKey, culture);
        var uri = new Uri(navigationManager.Uri)
            .GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
        var cultureEscaped = Uri.EscapeDataString(culture);
        var uriEscaped = Uri.EscapeDataString(uri);

        navigationManager.NavigateTo(
            $"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}",
            forceLoad: true);
    }

    public async Task<CultureInfo> GetStoredOrDefalutCultureInfoAsync()
    {
        var storedCultureName = await localStorageService.GetItemAsync<string?>(cultureOptions.LocalStorageKey);
        return !string.IsNullOrEmpty(storedCultureName)
            ? new CultureInfo(storedCultureName)
            : new CultureInfo(cultureOptions.DefaultCulture);
    }
}