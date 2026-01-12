using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.ApiService.Application.Queries.Menus;

/// <summary>
/// 检查父菜单是否存在查询
/// </summary>
public record CheckParentMenuExistsQuery(MenuId ParentId) : IQuery<bool>;

/// <summary>
/// 检查父菜单存在查询处理器
/// </summary>
public class CheckParentMenuExistsQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckParentMenuExistsQuery, bool>
{
    public async Task<bool> Handle(CheckParentMenuExistsQuery request, CancellationToken cancellationToken)
    {
        return await context.Menus
            .AnyAsync(x => x.Id == request.ParentId, cancellationToken);
    }
}
