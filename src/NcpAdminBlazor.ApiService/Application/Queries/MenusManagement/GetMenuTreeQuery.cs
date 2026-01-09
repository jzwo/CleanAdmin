using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.ApiService.Application.Queries.MenusManagement;

/// <summary>
/// 获取菜单树查询
/// </summary>
public record GetMenuTreeQuery : IQuery<List<MenuTreeNodeDto>>;

/// <summary>
/// 菜单树节点DTO
/// </summary>
public record MenuTreeNodeDto(
    MenuId MenuId,
    string MenuName,
    MenuType MenuType,
    MenuId? ParentId,
    string RoutePath,
    string? ComponentPath,
    string? Icon,
    int SortOrder,
    bool IsVisible,
    string? PermissionCode,
    MenuStatus Status,
    List<MenuTreeNodeDto> Children);

/// <summary>
/// 获取菜单树查询处理器
/// </summary>
public class GetMenuTreeQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetMenuTreeQuery, List<MenuTreeNodeDto>>
{
    public async Task<List<MenuTreeNodeDto>> Handle(GetMenuTreeQuery request, CancellationToken cancellationToken)
    {
        var allMenus = await context.Menus
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.ParentId)
            .ThenBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);

        // 构建树形结构
        var menuTree = BuildMenuTree(allMenus, null);
        return menuTree;
    }

    /// <summary>
    /// 递归构建菜单树
    /// </summary>
    private static List<MenuTreeNodeDto> BuildMenuTree(List<Menu> allMenus, MenuId? parentId)
    {
        return allMenus
            .Where(x => x.ParentId == parentId)
            .OrderBy(x => x.SortOrder)
            .Select(x => new MenuTreeNodeDto(
                x.Id,
                x.MenuName,
                x.MenuType,
                x.ParentId,
                x.RoutePath,
                x.ComponentPath,
                x.Icon,
                x.SortOrder,
                x.IsVisible,
                x.PermissionCode,
                x.Status,
                BuildMenuTree(allMenus, x.Id)))
            .ToList();
    }
}
