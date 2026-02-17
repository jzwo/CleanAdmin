using Microsoft.EntityFrameworkCore;
using CleanAdmin.Domain.AggregatesModel.MenuAggregate;

namespace CleanAdmin.ApiService.Application.Queries.Menus;

/// <summary>
/// 检查菜单排序号在同级是否重复查询
/// </summary>
public record CheckMenuSortOrderConflictQuery(
    int SortOrder,
    MenuId? ParentId,
    MenuId? ExcludeId = null) : IQuery<bool>;

/// <summary>
/// 检查菜单排序号冲突查询处理器
/// </summary>
public class CheckMenuSortOrderConflictQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckMenuSortOrderConflictQuery, bool>
{
    public async Task<bool> Handle(CheckMenuSortOrderConflictQuery request, CancellationToken cancellationToken)
    {
        var query = context.Menus.AsQueryable();

        query = request.ParentId is not null
            ? query.Where(x => x.ParentId == request.ParentId)
            : query.Where(x => x.ParentId == null);

        if (request.ExcludeId is not null)
        {
            query = query.Where(x => x.Id != request.ExcludeId);
        }

        return await query.AnyAsync(x => x.SortOrder == request.SortOrder, cancellationToken);
    }
}
