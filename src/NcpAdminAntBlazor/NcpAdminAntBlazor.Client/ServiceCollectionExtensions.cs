using NcpAdminAntBlazor.Client.Services;

namespace NcpAdminAntBlazor.Client;

public static class ServiceCollectionExtensions
{
    public static void AddClientServices(this IServiceCollection services)
    {
        services.AddAntDesign();
        services.AddScoped<ThemeService>();
    }
}