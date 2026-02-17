using CleanAdmin.Domain.Common;
using CleanAdmin.Domain.DomainEvents;

namespace CleanAdmin.Domain.AggregatesModel.MenuAggregate;

// 强类型ID定义
public partial record MenuId : IGuidStronglyTypedId;

/// <summary>
/// 菜单聚合根 - 管理菜单树结构、前端路由配置、通过权限码控制可见性
/// 
/// 关键不变式：
/// • 父菜单必须存在
/// • 不能形成循环引用
/// • 同级菜单排序号不能重复
/// • 权限码必须在系统定义中存在（可选校验）
/// </summary>
public class Menu : Entity<MenuId>, IAggregateRoot, ISoftDeletable
{
    protected Menu()
    {
    }

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string MenuName { get; private set; } = string.Empty;

    /// <summary>
    /// 菜单类型
    /// </summary>
    public MenuType MenuType { get; private set; }

    /// <summary>
    /// 父菜单ID
    /// </summary>
    public MenuId? ParentId { get; private set; }

    /// <summary>
    /// 路由路径
    /// </summary>
    public string RoutePath { get; private set; } = string.Empty;

    /// <summary>
    /// 组件路径
    /// </summary>
    public string? ComponentPath { get; private set; }

    /// <summary>
    /// 菜单图标
    /// </summary>
    public string? Icon { get; private set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; private set; }

    /// <summary>
    /// 是否为外链
    /// </summary>
    public bool IsExternal { get; private set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    public bool IsVisible { get; private set; } = true;

    /// <summary>
    /// 权限码
    /// </summary>
    public string? PermissionCode { get; private set; }

    /// <summary>
    /// 菜单状态
    /// </summary>
    public MenuStatus Status { get; private set; } = MenuStatus.Enabled;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// 软删除标记
    /// </summary>
    public Deleted IsDeleted { get; private set; } = false;

    /// <summary>
    /// 软删除时间
    /// </summary>
    public DeletedTime DeletedAt { get; private set; } = new(DateTimeOffset.MinValue);

    /// <summary>
    /// 创建菜单
    /// </summary>
    public Menu(
        string menuName,
        MenuType menuType,
        MenuId? parentId,
        string routePath,
        string? componentPath,
        string? icon,
        int sortOrder,
        string? permissionCode)
    {
        MenuName = menuName;
        MenuType = menuType;
        ParentId = parentId;
        RoutePath = routePath;
        ComponentPath = componentPath;
        Icon = icon;
        SortOrder = sortOrder;
        PermissionCode = permissionCode;
        IsExternal = false;
        IsVisible = true;
        Status = MenuStatus.Enabled;
        CreatedAt = DateTimeOffset.UtcNow;

        this.AddDomainEvent(new MenuCreatedDomainEvent(this));
    }

    /// <summary>
    /// 更新菜单信息
    /// </summary>
    public void Update(
        string menuName,
        string routePath,
        string? componentPath,
        string? icon,
        string? permissionCode)
    {
        MenuName = menuName;
        RoutePath = routePath;
        ComponentPath = componentPath;
        Icon = icon;
        PermissionCode = permissionCode;

        this.AddDomainEvent(new MenuUpdatedDomainEvent(this));
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    public void Delete()
    {
        if (IsDeleted)
            throw new KnownException("菜单已删除");

        IsDeleted = true;
        this.AddDomainEvent(new MenuDeletedDomainEvent(this));
    }

    /// <summary>
    /// 更新排序号
    /// </summary>
    public void UpdateSortOrder(int newSortOrder)
    {
        if (newSortOrder < 0)
            throw new KnownException("排序号不能为负数");

        SortOrder = newSortOrder;
        this.AddDomainEvent(new MenuSortOrderChangedDomainEvent(this));
    }

    /// <summary>
    /// 显示菜单
    /// </summary>
    public void Show()
    {
        if (IsVisible)
            throw new KnownException("菜单已显示");

        IsVisible = true;
    }

    /// <summary>
    /// 隐藏菜单
    /// </summary>
    public void Hide()
    {
        if (!IsVisible)
            throw new KnownException("菜单已隐藏");

        IsVisible = false;
    }
}
