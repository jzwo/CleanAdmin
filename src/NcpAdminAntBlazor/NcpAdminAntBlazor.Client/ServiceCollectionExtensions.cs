using Bit.Butil;
using Blazilla.Extensions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
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
    /// 添加 Kiota API 客户端
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="baseUrl">API 基础地址</param>
    /// <param name="configureClient">配置 HttpClient 管道的委托（可选），用于添加自定义 Handler</param>
    /// <returns>服务集合（用于链式调用）</returns>
    public static IServiceCollection AddKiotaClient(
        this IServiceCollection services,
        string baseUrl,
        Action<IHttpClientBuilder>? configureClient = null)
    {
        // 1. 注册认证提供程序，默认使用匿名认证提供程序
        services.AddScoped<IAuthenticationProvider, AnonymousAuthenticationProvider>();

        // 2. 注册 Kiota 核心服务
        services.AddKiotaHandlers();

        // 3. 注册 Factory 和 HttpClient
        var builder = services.AddHttpClient<ApiClientFactory>((_, client) =>
            {
                // 设置基础地址和其他 HttpClient 配置
                client.BaseAddress = new Uri(baseUrl);
            })
            .AttachKiotaHandlers(); // 挂载 Kiota 必须的 Handler

        // 4. 【关键】执行外部传入的配置逻辑
        // 这里允许调用者挂载 1个、2个 或 N个 任意的 Handler
        // 例如: 401 拦截器、日志记录器、重试策略等
        configureClient?.Invoke(builder);

        // 5. 注册最终生成的 Client
        services.AddTransient(sp => sp.GetRequiredService<ApiClientFactory>().GetClient());

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