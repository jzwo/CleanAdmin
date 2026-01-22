using System.ComponentModel.DataAnnotations;

namespace NcpAdminAntBlazor.Options;

/// <summary>
/// API 服务配置选项
/// </summary>
public class ApiServiceOptions
{
    /// <summary>
    /// 认证 API 路径前缀（不带尾部斜杠）
    /// </summary>
    [Required]
    public string AuthPathPrefix { get; init; } = "/api/auth";

    /// <summary>
    /// API 服务地址
    /// </summary>
    [Required]
    public string ServiceAddress { get; init; } = "https+http://apiservice";
}