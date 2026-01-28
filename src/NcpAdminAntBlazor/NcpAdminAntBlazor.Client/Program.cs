using BitzArt.Blazor.Cookies;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NcpAdminAntBlazor.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddAuthenticationStateDeserialization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddClientServices();
builder.AddBlazorCookies();

builder.Services.AddClientAuthentication();
builder.Services.AddKiotaClient(new(builder.HostEnvironment.BaseAddress));

var host = builder.Build();

await host.SetCulture();

await host.RunAsync();