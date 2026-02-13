using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;

namespace NcpAdminBlazor.ApiService.Application.Queries.Menus;

/// <summary>
/// 获取当前用户菜单查询
/// </summary>
public record GetCurrentUserMenusQuery(UserId UserId) : IQuery<List<UserMenuTreeNodeDto>>;

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
/// 获取当前用户菜单查询处理器
/// </summary>
public class GetCurrentUserMenusQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetCurrentUserMenusQuery, List<UserMenuTreeNodeDto>>
{
    public async Task<List<UserMenuTreeNodeDto>> Handle(GetCurrentUserMenusQuery request, CancellationToken cancellationToken)
    {
        // 获取用户角色
        var user = await context.Users
            .Where(u => u.Id == request.UserId && !u.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KnownException($"用户不存在，UserId = {request.UserId}");

        // 获取用户所有权限码
        var userPermissions = new HashSet<string>();
        if (user.AssignedRoleIds.Any())
        {
            var rolePermissions = await context.Roles
                .Where(r => user.AssignedRoleIds.Contains(r.Id) && !r.IsDeleted && !r.IsDisabled)
                .SelectMany(r => r.AssignedPermissionCodes)
                .Distinct()
                .ToListAsync(cancellationToken);
            
            foreach (var permission in rolePermissions)
            {
                userPermissions.Add(permission);
            }
        }

        // 获取所有可见的已启用菜单
        var allMenus = await context.Menus
            .Where(x => !x.IsDeleted && x.IsVisible && x.Status == MenuStatus.Enabled)
            .OrderBy(x => x.ParentId)
            .ThenBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);

        // 过滤用户有直接权限的菜单
        var directAccessibleMenus = allMenus
            .Where(m => string.IsNullOrEmpty(m.PermissionCode) || userPermissions.Contains(m.PermissionCode))
            .ToList();

        // 获取这些菜单的所有祖先节点（父菜单、祖父菜单等）
        var accessibleMenuIds = new HashSet<MenuId>(directAccessibleMenus.Select(m => m.Id));
        var menusToInclude = new HashSet<MenuId>(accessibleMenuIds);
        
        foreach (var menu in directAccessibleMenus)
        {
            var parentId = menu.ParentId;
            while (parentId != null)
            {
                menusToInclude.Add(parentId);
                var parentMenu = allMenus.FirstOrDefault(m => m.Id == parentId);
                parentId = parentMenu?.ParentId;
            }
        }

        // 最终的可访问菜单列表（包含祖先节点）
        var finalMenus = allMenus
            .Where(m => menusToInclude.Contains(m.Id))
            .ToList();

        // 构建树形结构
        var menuTree = BuildUserMenuTree(finalMenus, null);
        
        // 过滤掉没有子菜单的目录类型节点
        var filteredTree = FilterEmptyDirectories(menuTree);
        
        return filteredTree;
    }

    /// <summary>
    /// 递归过滤掉没有子菜单的目录类型节点
    /// </summary>
    private static List<UserMenuTreeNodeDto> FilterEmptyDirectories(List<UserMenuTreeNodeDto> nodes)
    {
        var result = new List<UserMenuTreeNodeDto>();
        
        foreach (var node in nodes)
        {
            // 递归处理子节点
            var filteredChildren = node.Children.Count > 0 
                ? FilterEmptyDirectories(node.Children) 
                : new List<UserMenuTreeNodeDto>();
            
            // 如果是目录类型
            if (node.MenuType == MenuType.Directory)
            {
                // 只有当目录有子菜单时才包含
                if (filteredChildren.Count > 0)
                {
                    result.Add(node with { Children = filteredChildren });
                }
                // 空目录直接跳过
            }
            else
            {
                // 非目录类型（Menu、Button），直接添加
                result.Add(node with { Children = filteredChildren });
            }
        }
        
        return result;
    }

    /// <summary>
    /// 递归构建用户菜单树
    /// </summary>
    private static List<UserMenuTreeNodeDto> BuildUserMenuTree(List<Menu> allMenus, MenuId? parentId)
    {
        var children = allMenus
            .Where(x => x.ParentId == parentId)
            .OrderBy(x => x.SortOrder)
            .Select(x => new UserMenuTreeNodeDto(
                x.Id,
                x.MenuName,
                x.MenuType,
                x.RoutePath,
                x.ComponentPath,
                x.Icon,
                x.SortOrder,
                BuildUserMenuTree(allMenus, x.Id)))
            .ToList();

        return children;
    }
}


