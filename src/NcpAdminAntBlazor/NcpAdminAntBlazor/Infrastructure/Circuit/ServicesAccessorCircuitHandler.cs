 using Microsoft.AspNetCore.Components.Server.Circuits;

namespace NcpAdminAntBlazor.Infrastructure.Circuit;

/// <summary>
/// Circuit 处理器，用于设置服务访问器
/// </summary>
/// <param name="services">服务提供程序</param>
/// <param name="servicesAccessor">服务访问器</param>
public class ServicesAccessorCircuitHandler(
    IServiceProvider services,
    CircuitServicesAccessor servicesAccessor)
    : CircuitHandler
{
    public override Func<CircuitInboundActivityContext, Task> CreateInboundActivityHandler(
        Func<CircuitInboundActivityContext, Task> next) =>
        async context =>
        {
            servicesAccessor.Services = services;
            await next(context);
            servicesAccessor.Services = null;
        };
}
