using System.Net;
using Microsoft.AspNetCore.Components;
using NcpAdminAntBlazor.Client.Extensions;

namespace NcpAdminAntBlazor.Client.Infrastructure.Http;

/// <summary>
/// HTTP 消息处理器，用于处理 401 未授权响应并重定向到登录页面
/// </summary>
/// <param name="navigationManager">导航管理器</param>
public class ClientUnauthorizedHandler(NavigationManager navigationManager) : DelegatingHandler
{
    /// <summary>
    /// 发送 HTTP 请求并处理 401 未授权响应
    /// </summary>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }
        
        navigationManager.NavigateToLogin();

        return response;
    }
}
