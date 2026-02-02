namespace NcpAdminAntBlazor.Client.Components;

/// <summary>
/// PForm 接口，用于 PFormItem 与 PForm 之间的通信
/// </summary>
public interface IPForm
{
    /// <summary>
    /// 是否启用折叠功能
    /// </summary>
    bool Collapsible { get; }

    /// <summary>
    /// 是否折叠状态
    /// </summary>
    bool Collapsed { get; }

    /// <summary>
    /// 折叠时显示的字段数量
    /// </summary>
    int CollapseCount { get; }

    /// <summary>
    /// 注册表单字段
    /// </summary>
    void AddField(PFormItem field);

    /// <summary>
    /// 移除表单字段
    /// </summary>
    void RemoveField(PFormItem field);

    /// <summary>
    /// 判断字段是否应该显示
    /// </summary>
    bool ShouldShowField(PFormItem field);

    /// <summary>
    /// 已注册的字段数量
    /// </summary>
    int FieldCount { get; }
}






