using BitzArt.Blazor.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using NcpAdminAntBlazor.Client;
using NcpAdminAntBlazor.Client.Services;
using NcpAdminAntBlazor.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddClientServices();
builder.AddBlazorCookies();

builder.Services.AddClientAuthentication();

#pragma warning disable S1075
builder.Services.AddKiotaClient(new("https+http://apiservice"));
#pragma warning restore S1075

builder.Services.AddOptions<RequestLocalizationOptions>()
    .Configure<IOptions<CultureOptions>>((locOptions, cultureConfig) =>
    {
        var settings = cultureConfig.Value;
        locOptions.SetDefaultCulture(settings.DefaultCulture);
        locOptions.AddSupportedCultures(settings.SupportedCultures);
        locOptions.AddSupportedUICultures(settings.SupportedCultures);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapDefaultEndpoints();

app.MapStaticAssets();

app.UseRequestLocalization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(NcpAdminAntBlazor.Client._Imports).Assembly)
    .AllowAnonymous();

app.MapGet("/Culture/Set", (string? culture, string redirectUri, HttpContext httpContext) =>
{
    if (!string.IsNullOrEmpty(culture))
    {
        httpContext.Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(
                new RequestCulture(culture, culture)));
    }

    return Results.LocalRedirect(redirectUri);
});

#pragma warning disable S6966
app.Run();
#pragma warning restore S6966