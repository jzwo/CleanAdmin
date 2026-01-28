using Bit.Butil;
using Blazilla.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using NcpAdminAntBlazor.Client.ApiSdk;
using NcpAdminAntBlazor.Client.Infrastructure.Http;
using NcpAdminAntBlazor.Client.Services;

namespace NcpAdminAntBlazor.Client;

public static class ServiceCollectionExtensions
{
    public static void AddClientServices(this IServiceCollection services)
    {
        services.AddAntDesign();
        services.AddBitButilServices();
        services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
        services.AddScoped<ThemeService>();
        services.Configure<CultureOptions>(options =>
        {
            options.LocalStorageKey = "BlazorCulture";
            options.DefaultCulture = "zh-CN";
            options.SupportedCultures = ["zh-CN", "en-US"];
        });
        services.AddScoped<ICultureService, CultureService>();
        services.AddSingleton<IMenuService, MenuService>();
        services.AddValidatorsFromAssemblyContaining<Program>();
    }

    /// <summary>
    /// 注册认证相关的基础服务 (Storage, AuthState, Refresher)
    /// </summary>
    public static IServiceCollection AddClientAuthentication(this IServiceCollection services)
    {
        services.AddScoped<IUserTokenStore, UserTokenStore>();
        services.AddScoped<IUserTokenRefresher, UserTokenRefresher>();

        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();

        services.AddScoped<JwtAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp =>
            sp.GetRequiredService<JwtAuthStateProvider>());

        return services;
    }

    /// <summary>
    /// 添加 Kiota API 客户端
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="baseUrl">API 基础地址</param>
    /// <returns>服务集合（用于链式调用）</returns>
    public static IServiceCollection AddKiotaClient(
        this IServiceCollection services,
        Uri baseUrl)
    {
        services.AddScoped<IAccessTokenProvider, AccessTokenProvider>();
        services.AddScoped<BaseBearerTokenAuthenticationProvider>();
        services.AddScoped<IAuthenticationProvider, BearerTokenAuthenticationProvider>();
        services.AddTransient<ClientUnauthorizedHandler>();
        services.AddKiotaHandlers();
        var httpClientBuilder = services.AddHttpClient<ApiClientFactory>((_, client) =>
            {
                client.BaseAddress = baseUrl;
            })
            .AttachKiotaHandlers();
        httpClientBuilder.AddHttpMessageHandler<ClientUnauthorizedHandler>();

        services.AddTransient<ApiClient>(sp => sp.GetRequiredService<ApiClientFactory>().GetClient());
        services.AddTransient<Lazy<ApiClient>>(sp => new Lazy<ApiClient>(sp.GetRequiredService<ApiClient>));
        return services;
    }


    /// <summary>
    /// 添加 Kiota 处理器到服务集合
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/> 服务集合</param>
    /// <returns><see cref="IServiceCollection"/> 用于链式调用</returns>
    /// <remarks>处理器通过 <see cref="AttachKiotaHandlers(IHttpClientBuilder)"/> 调用附加到 HTTP 客户端，需要预先在 DI 中注册</remarks>
    private static void AddKiotaHandlers(this IServiceCollection services)
    {
        // 从 Kiota 客户端工厂动态加载处理器
        var kiotaHandlers = KiotaClientFactory.GetDefaultHandlerActivatableTypes();

        // 在 DI 容器中注册处理器
        foreach (var handler in kiotaHandlers)
        {
            services.AddTransient(handler);
        }
    }

    /// <summary>
    /// 将 Kiota 处理器附加到 HTTP 客户端构建器
    /// </summary>
    /// <param name="builder">HTTP 客户端构建器</param>
    /// <returns>HTTP 客户端构建器用于链式调用</returns>
    /// <remarks>
    /// 需要通过 <see cref="AddKiotaHandlers(IServiceCollection)"/> 预先在 DI 中注册处理器
    /// </remarks>
    private static IHttpClientBuilder AttachKiotaHandlers(this IHttpClientBuilder builder)
    {
        // 从 Kiota 客户端工厂动态加载处理器
        var kiotaHandlers = KiotaClientFactory.GetDefaultHandlerActivatableTypes();

        // 将处理器附加到 HTTP 客户端构建器
        foreach (var handler in kiotaHandlers)
        {
            builder.AddHttpMessageHandler(sp => (DelegatingHandler)sp.GetRequiredService(handler));
        }

        return builder;
    }
}