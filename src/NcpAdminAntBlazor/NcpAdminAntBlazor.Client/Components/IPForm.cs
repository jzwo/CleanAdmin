using AntDesign;
using OneOf;

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

    #region Item Span Properties

    /// <summary>
    /// 子项栅格占位格数 (1-24)
    /// </summary>
    OneOf<string, int>? ItemSpan { get; }

    /// <summary>
    /// 子项 &lt;576px 响应式栅格
    /// </summary>
    OneOf<int, EmbeddedProperty>? ItemXs { get; }

    /// <summary>
    /// 子项 ≥576px 响应式栅格
    /// </summary>
    OneOf<int, EmbeddedProperty>? ItemSm { get; }

    /// <summary>
    /// 子项 ≥768px 响应式栅格
    /// </summary>
    OneOf<int, EmbeddedProperty>? ItemMd { get; }

    /// <summary>
    /// 子项 ≥992px 响应式栅格
    /// </summary>
    OneOf<int, EmbeddedProperty>? ItemLg { get; }

    /// <summary>
    /// 子项 ≥1200px 响应式栅格
    /// </summary>
    OneOf<int, EmbeddedProperty>? ItemXl { get; }

    /// <summary>
    /// 子项 ≥1600px 响应式栅格
    /// </summary>
    OneOf<int, EmbeddedProperty>? ItemXxl { get; }

    #endregion
}






