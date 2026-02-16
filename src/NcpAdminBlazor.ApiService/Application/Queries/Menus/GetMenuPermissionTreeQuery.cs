using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.ApiService.Application.Queries.Menus;

/// <summary>
/// 获取菜单权限树查询
/// </summary>
public sealed record GetMenuPermissionTreeQuery : IQuery<List<MenuPermissionTreeNodeDto>>;

public sealed record MenuPermissionTreeNodeDto(
    MenuId MenuId,
    string MenuName,
    string? Icon,
    string? PermissionCode,
    List<MenuPermissionTreeNodeDto> Children);

public sealed class GetMenuPermissionTreeQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetMenuPermissionTreeQuery, List<MenuPermissionTreeNodeDto>>
{
    public async Task<List<MenuPermissionTreeNodeDto>> Handle(GetMenuPermissionTreeQuery request,
        CancellationToken cancellationToken)
    {
        var menus = await context.Menus
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.ParentId)
            .ThenBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);

        var childrenByParent = menus
            .Where(x => x.ParentId != null)
            .GroupBy(x => x.ParentId!)
            .ToDictionary(x => x.Key, x => x.OrderBy(menu => menu.SortOrder).ToList());

        var roots = menus
            .Where(x => x.ParentId == null)
            .OrderBy(x => x.SortOrder)
            .ToList();

        return roots.Select(root => BuildNode(root, childrenByParent)).ToList();
    }

    private static MenuPermissionTreeNodeDto BuildNode(
        Menu menu,
        IReadOnlyDictionary<MenuId, List<Menu>> childrenByParent)
    {
        var children = new List<MenuPermissionTreeNodeDto>();
        if (childrenByParent.TryGetValue(menu.Id, out var childMenus))
        {
            children.AddRange(childMenus.Select(childMenu => BuildNode(childMenu, childrenByParent)));
        }

        return new MenuPermissionTreeNodeDto(
            menu.Id,
            menu.MenuName,
            menu.Icon,
            string.IsNullOrWhiteSpace(menu.PermissionCode) ? null : menu.PermissionCode,
            children);
    }
}