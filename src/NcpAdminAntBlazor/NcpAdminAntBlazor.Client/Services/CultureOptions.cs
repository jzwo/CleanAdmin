namespace NcpAdminAntBlazor.Client.Services;

/// <summary>
/// 文化配置选项，包含支持的文化和默认文化
/// </summary>
public interface ICultureOptions
{
    public string LocalStorageKey { get; }
    public string[] SupportedCultures { get; }
    public string DefaultCulture { get; }
}

public class CultureOptions : ICultureOptions
{
    private const string LocalStorageKeyValue = "BlazorCulture";
    private const string DefaultCultureValue = "zh-CN";
    private static readonly string[] SupportedCulturesValue = ["zh-CN", "en-US"];

    public string LocalStorageKey => LocalStorageKeyValue;
    public string[] SupportedCultures => SupportedCulturesValue;

    public string DefaultCulture => DefaultCultureValue;
}