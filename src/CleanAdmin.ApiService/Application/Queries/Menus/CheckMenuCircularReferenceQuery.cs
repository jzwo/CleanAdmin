using Microsoft.EntityFrameworkCore;
using CleanAdmin.Domain.AggregatesModel.MenuAggregate;

namespace CleanAdmin.ApiService.Application.Queries.Menus;

/// <summary>
/// 检查循环引用查询
/// </summary>
public record CheckMenuCircularReferenceQuery(MenuId MenuId, MenuId? NewParentId) : IQuery<bool>;

/// <summary>
/// 检查菜单循环引用查询处理器
/// </summary>
public class CheckMenuCircularReferenceQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckMenuCircularReferenceQuery, bool>
{
    public async Task<bool> Handle(CheckMenuCircularReferenceQuery request, CancellationToken cancellationToken)
    {
        if (request.NewParentId is null || request.MenuId == request.NewParentId)
            return false;

        var menu = await context.Menus
            .FirstOrDefaultAsync(x => x.Id == request.NewParentId, cancellationToken);

        if (menu == null)
            return false;

        // 递归检查所有父菜单
        var parentId = menu.ParentId;
        while (parentId is not null)
        {
            if (parentId == request.MenuId)
                return true;

            var parent = await context.Menus
                .FirstOrDefaultAsync(x => x.Id == parentId, cancellationToken);

            if (parent == null)
                break;

            parentId = parent.ParentId;
        }

        return false;
    }
}
