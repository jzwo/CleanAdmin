using System.Net.Http.Headers;
using BitzArt.Blazor.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions.Authentication;
using NcpAdminAntBlazor;
using NcpAdminAntBlazor.Client;
using NcpAdminAntBlazor.Client.Services;
using NcpAdminAntBlazor.Components;
using NcpAdminAntBlazor.Extensions;
using NcpAdminAntBlazor.Infrastructure.Http;
using NcpAdminAntBlazor.Middleware;
using NcpAdminAntBlazor.Options;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// 配置选项
var apiSettingsSection = builder.Configuration.GetSection("ApiServiceSettings");
var apiServiceOptions = apiSettingsSection.Get<ApiServiceOptions>() ?? new ApiServiceOptions();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddOptions<ApiServiceOptions>().Bind(apiSettingsSection).ValidateDataAnnotations().ValidateOnStart();

builder.Services.AddHybridCache();

// 添加认证服务
builder.Services.AddCookieAuthenticationWithRefresh();
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// 添加 Circuit 服务访问器
builder.Services.AddCircuitServicesAccessor();

// 注册服务端 HTTP 消息处理器
builder.Services.AddScoped<ServerUnauthorizedHandler>();

// 添加 Kiota API 客户端
builder.Services.AddKiotaClient(
    apiServiceOptions.ServiceAddress,
    clientBuilder => { clientBuilder.AddHttpMessageHandler<ServerUnauthorizedHandler>(); });
builder.Services.AddScoped<IAccessTokenProvider, KiotaAccessTokenProvider>();
builder.Services.AddScoped<BaseBearerTokenAuthenticationProvider>();
// 在服务端使用 Bearer Token 认证提供器
builder.Services.AddScoped<IAuthenticationProvider, BearerTokenAuthenticationProvider>();

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

// 添加 YARP 反向代理
builder.Services.AddHttpForwarder();

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

app.UseAuthentication();

app.UseAuthorization();

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

// 【Token 清理中间件】在认证之后，YARP之前
// 用于处理 401 响应，自动清除过期的 Token 和 Cookie
app.UseMiddleware<TokenCleanupMiddleware>();

// 映射 API 转发
app.MapForwarder("/api/{**catch-all}", apiServiceOptions.ServiceAddress, transformBuilder =>
    {
        transformBuilder.AddRequestTransform(transformContext =>
        {
            if (transformContext.HttpContext.Items.TryGetValue(
                    AntBlazorConstants.HttpContextItems.AccessToken,
                    out var tokenObj) && tokenObj is string token)
            {
                transformContext.ProxyRequest.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return ValueTask.CompletedTask;
        });
    })
    .RequireAuthorization(policyBuilder =>
    {
        policyBuilder.RequireAssertion(context =>
        {
            if (context.Resource is not HttpContext httpContext)
                return context.User.Identity?.IsAuthenticated == true;

            // 认证端点允许匿名访问
            if (httpContext.Request.Path.StartsWithSegments(apiServiceOptions.AuthPathPrefix))
                return true;

            return context.User.Identity?.IsAuthenticated == true;
        });
    });

#pragma warning disable S6966
app.Run();
#pragma warning restore S6966