using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.ApiService.Application.Queries.MenusManagement;

/// <summary>
/// 检查菜单是否有子菜单查询
/// </summary>
public record CheckMenuHasChildrenQuery(MenuId MenuId) : IQuery<bool>;

/// <summary>
/// 检查菜单有子菜单查询处理器
/// </summary>
public class CheckMenuHasChildrenQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckMenuHasChildrenQuery, bool>
{
    public async Task<bool> Handle(CheckMenuHasChildrenQuery request, CancellationToken cancellationToken)
    {
        return await context.Menus
            .AnyAsync(x => x.ParentId == request.MenuId, cancellationToken);
    }
}
