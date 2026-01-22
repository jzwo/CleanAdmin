using Microsoft.AspNetCore.Components.Server.Circuits;
using NcpAdminAntBlazor.Infrastructure.Circuit;

namespace NcpAdminAntBlazor.Extensions;

/// <summary>
/// Circuit 服务访问器扩展方法
/// </summary>
public static class CircuitServicesServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Circuit 服务访问器
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合（用于链式调用）</returns>
    public static IServiceCollection AddCircuitServicesAccessor(this IServiceCollection services)
    {
        services.AddScoped<CircuitServicesAccessor>();
        services.AddScoped<CircuitHandler, ServicesAccessorCircuitHandler>();

        return services;
    }
}
