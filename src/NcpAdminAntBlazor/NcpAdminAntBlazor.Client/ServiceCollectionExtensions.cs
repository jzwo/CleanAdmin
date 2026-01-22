using Bit.Butil;
using Blazilla.Extensions;
using NcpAdminAntBlazor.Client.Services;

namespace NcpAdminAntBlazor.Client;

public static class ServiceCollectionExtensions
{
    public static void AddClientServices(this IServiceCollection services)
    {
        services.AddAntDesign();
        services.AddBitButilServices();
        services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
        services.AddScoped<ThemeService>();
        services.Configure<CultureOptions>(options =>
        {
            options.LocalStorageKey = "BlazorCulture";
            options.DefaultCulture = "zh-CN";
            options.SupportedCultures = ["zh-CN", "en-US"];
        });
        services.AddScoped<ICultureService, CultureService>();
        services.AddSingleton<IMenuService, MenuService>();
        services.AddValidatorsFromAssemblyContaining<Program>();
    }
}