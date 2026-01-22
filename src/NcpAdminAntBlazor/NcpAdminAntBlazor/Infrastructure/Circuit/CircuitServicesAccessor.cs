namespace NcpAdminAntBlazor.Infrastructure.Circuit;

/// <summary>
/// Circuit 服务访问器
/// 用于在 Blazor Server Circuit 生命周期内访问服务
/// </summary>
public class CircuitServicesAccessor
{
    private static readonly AsyncLocal<IServiceProvider> BlazorServices = new();

    public IServiceProvider? Services
    {
        get => BlazorServices.Value;
        set => BlazorServices.Value = value!;
    }
}
