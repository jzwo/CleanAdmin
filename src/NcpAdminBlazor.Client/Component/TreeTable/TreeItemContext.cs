namespace NcpAdminBlazor.Client.Component.TreeTable;

public class TreeItemContext<T>
{
    public required T Item { get; set; }
    public int Level { get; set; }
    public bool HasChildren { get; set; }
    public bool IsExpanded { get; set; }
    public bool IsVisible { get; set; }
    public int IndentSize { get; set; }

    public  TreeItemContext<T>? Parent { get; set; }
    
    // 存储子节点的包装对象，用于级联更新
    public List<TreeItemContext<T>> ChildContextsWrappers { get; set; } = [];

    // 这是一个回调委托，用于触发 TreeTable 的刷新逻辑
    public Action<TreeItemContext<T>>? ToggleAction { get; set; }

    public void Toggle()
    {
        ToggleAction?.Invoke(this);
    }
}