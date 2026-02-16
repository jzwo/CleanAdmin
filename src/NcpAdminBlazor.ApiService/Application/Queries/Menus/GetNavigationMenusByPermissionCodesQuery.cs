using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.ApiService.Application.Queries.Menus;

/// <summary>
/// 根据权限点获取导航菜单查询
/// </summary>
public record GetNavigationMenusByPermissionCodesQuery(List<string> PermissionCodes) : IQuery<List<UserMenuTreeNodeDto>>;

public class GetNavigationMenusByPermissionCodesQueryValidator : AbstractValidator<GetNavigationMenusByPermissionCodesQuery>
{
    public GetNavigationMenusByPermissionCodesQueryValidator()
    {
        RuleFor(x => x.PermissionCodes)
            .NotNull().WithMessage("权限点不能为空");
    }
}

/// <summary>
/// 用户菜单树节点DTO
/// </summary>
public record UserMenuTreeNodeDto(
    MenuId MenuId,
    string MenuName,
    MenuType MenuType,
    string RoutePath,
    string? ComponentPath,
    string? Icon,
    int SortOrder,
    List<UserMenuTreeNodeDto> Children);

/// <summary>
/// 根据权限点获取导航菜单查询处理器
/// </summary>
public class GetNavigationMenusByPermissionCodesQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetNavigationMenusByPermissionCodesQuery, List<UserMenuTreeNodeDto>>
{
    public async Task<List<UserMenuTreeNodeDto>> Handle(GetNavigationMenusByPermissionCodesQuery request, CancellationToken cancellationToken)
    {
        var userPermissions = request.PermissionCodes
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet(StringComparer.Ordinal);

        var allMenus = await context.Menus
            .AsNoTracking()
            .Where(x => x.IsVisible && x.Status == MenuStatus.Enabled)
            .Where(x => x.MenuType == MenuType.Directory || x.MenuType == MenuType.Menu)
            .OrderBy(x => x.ParentId)
            .ThenBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);

        var directAccessibleMenus = allMenus
            .Where(m => string.IsNullOrEmpty(m.PermissionCode) || userPermissions.Contains(m.PermissionCode))
            .ToList();

        var menusToInclude = new HashSet<MenuId>(directAccessibleMenus.Select(m => m.Id));
        var menuById = allMenus.ToDictionary(m => m.Id);

        foreach (var parentId in directAccessibleMenus.Select(menu => menu.ParentId))
        {
            var currentParentId = parentId;
            while (currentParentId != null)
            {
                if (!menusToInclude.Add(currentParentId))
                {
                    break;
                }

                if (!menuById.TryGetValue(currentParentId, out var parentMenu))
                {
                    break;
                }

                currentParentId = parentMenu.ParentId;
            }
        }

        var accessibleMenus = allMenus
            .Where(m => menusToInclude.Contains(m.Id))
            .ToList();

        var rootMenus = accessibleMenus
            .Where(x => x.ParentId == null)
            .OrderBy(x => x.SortOrder)
            .ToList();

        var childrenByParent = accessibleMenus
            .Where(x => x.ParentId != null)
            .GroupBy(x => x.ParentId!)
            .ToDictionary(x => x.Key, x => x.OrderBy(menu => menu.SortOrder).ToList());

        return BuildFilteredMenuTree(rootMenus, childrenByParent);
    }

    private static List<UserMenuTreeNodeDto> BuildFilteredMenuTree(
        List<Menu> currentLevelMenus,
        IReadOnlyDictionary<MenuId, List<Menu>> childrenByParent)
    {
        var result = new List<UserMenuTreeNodeDto>(currentLevelMenus.Count);

        foreach (var child in currentLevelMenus)
        {
            var filteredChildren = childrenByParent.TryGetValue(child.Id, out var childMenus)
                ? BuildFilteredMenuTree(childMenus, childrenByParent)
                : [];

            if (child.MenuType == MenuType.Directory && filteredChildren.Count == 0)
            {
                continue;
            }

            result.Add(new UserMenuTreeNodeDto(
                child.Id,
                child.MenuName,
                child.MenuType,
                child.RoutePath,
                child.ComponentPath,
                child.Icon,
                child.SortOrder,
                filteredChildren));
        }

        return result;
    }
}


