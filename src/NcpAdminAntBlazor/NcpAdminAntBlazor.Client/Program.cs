using BitzArt.Blazor.Cookies;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NcpAdminAntBlazor.Client;
using NcpAdminAntBlazor.Client.Infrastructure.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddAuthenticationStateDeserialization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddClientServices();
builder.AddBlazorCookies();

// 注册 HTTP 消息处理器
builder.Services.AddScoped<ClientUnauthorizedHandler>();
// 添加 Kiota API 客户端
builder.Services.AddKiotaClient(
    builder.HostEnvironment.BaseAddress,
    clientBuilder =>
    {
        // 挂载 401 未授权处理器
        clientBuilder.AddHttpMessageHandler<ClientUnauthorizedHandler>();
    });

var host = builder.Build();

await host.SetCulture();

await host.RunAsync();