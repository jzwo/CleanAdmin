using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using CleanAdmin.Web.Client.ApiSdk;

namespace CleanAdmin.Web.Client.Infrastructure.Http;

/// <summary>
/// API 客户端工厂，负责创建 ApiClient 实例
/// </summary>
/// <param name="httpClient">HTTP 客户端实例</param>
/// <param name="authenticationProvider">认证提供程序</param>
public class ApiClientFactory(HttpClient httpClient, IAuthenticationProvider authenticationProvider)
{
    /// <summary>
    /// 获取 ApiClient 实例
    /// </summary>
    /// <returns>配置好的 ApiClient 实例</returns>
    public ApiClient GetClient()
    {
        return new ApiClient(new HttpClientRequestAdapter(
            authenticationProvider,
            httpClient: httpClient));
    }
}
