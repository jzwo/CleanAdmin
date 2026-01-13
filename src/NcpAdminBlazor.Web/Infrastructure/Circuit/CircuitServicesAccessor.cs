namespace NcpAdminBlazor.Web.Infrastructure.Circuit;

public class CircuitServicesAccessor
{
    private static readonly AsyncLocal<IServiceProvider> BlazorServices = new();

    public IServiceProvider? Services
    {
        get => BlazorServices.Value;
        set => BlazorServices.Value = value!;
    }
}
