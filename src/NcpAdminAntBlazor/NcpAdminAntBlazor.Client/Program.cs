using BitzArt.Blazor.Cookies;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NcpAdminAntBlazor.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddClientServices();
builder.AddBlazorCookies();

var host = builder.Build();

await host.SetCulture();

await host.RunAsync();