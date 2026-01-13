using Microsoft.AspNetCore.Components.Server.Circuits;
using NcpAdminBlazor.Web.Infrastructure.Circuit;

namespace NcpAdminBlazor.Web.Extensions;

public static class CircuitServicesServiceCollectionExtensions
{
    public static IServiceCollection AddCircuitServicesAccessor(this IServiceCollection services)
    {
        services.AddScoped<CircuitServicesAccessor>();
        services.AddScoped<CircuitHandler, ServicesAccessorCircuitHandler>();

        return services;
    }
}