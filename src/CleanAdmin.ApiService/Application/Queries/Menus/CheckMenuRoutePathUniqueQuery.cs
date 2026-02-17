using Microsoft.EntityFrameworkCore;
using CleanAdmin.Domain.AggregatesModel.MenuAggregate;

namespace CleanAdmin.ApiService.Application.Queries.Menus;

/// <summary>
/// 检查菜单路由路径是否唯一查询
/// </summary>
public record CheckMenuRoutePathUniqueQuery(
    string RoutePath,
    MenuId? ExcludeId = null) : IQuery<bool>;

/// <summary>
/// 检查菜单路由路径唯一查询处理器
/// </summary>
public class CheckMenuRoutePathUniqueQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckMenuRoutePathUniqueQuery, bool>
{
    public async Task<bool> Handle(CheckMenuRoutePathUniqueQuery request, CancellationToken cancellationToken)
    {
        var query = context.Menus.AsQueryable();

        if (request.ExcludeId is not null)
        {
            query = query.Where(x => x.Id != request.ExcludeId);
        }

        return await query.AnyAsync(x => x.RoutePath == request.RoutePath, cancellationToken);
    }
}
