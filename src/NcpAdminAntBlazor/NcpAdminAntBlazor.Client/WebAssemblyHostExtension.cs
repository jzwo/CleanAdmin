using System.Globalization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NcpAdminAntBlazor.Client.Services;

namespace NcpAdminAntBlazor.Client;

public static class WebAssemblyHostExtension
{
    /// <summary>
    /// set culture from local storage or default culture
    /// </summary>
    /// <param name="host"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task SetCulture(this WebAssemblyHost host)
    {
        var cultureService = host.Services.GetRequiredService<ICultureService>();
        var culture = await cultureService.GetStoredOrDefaultCultureInfoAsync();
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}