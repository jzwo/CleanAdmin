using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.ApiService.Application.Queries.Menus;

/// <summary>
/// 检查菜单名称在同级是否重复查询
/// </summary>
public record CheckMenuNameConflictQuery(
    string MenuName,
    MenuId? ParentId,
    MenuId? ExcludeId = null) : IQuery<bool>;

/// <summary>
/// 检查菜单名称冲突查询处理器
/// </summary>
public class CheckMenuNameConflictQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckMenuNameConflictQuery, bool>
{
    public async Task<bool> Handle(CheckMenuNameConflictQuery request, CancellationToken cancellationToken)
    {
        var query = context.Menus.AsQueryable();

        query = request.ParentId is not null
            ? query.Where(x => x.ParentId == request.ParentId)
            : query.Where(x => x.ParentId == null);

        if (request.ExcludeId is not null)
        {
            query = query.Where(x => x.Id != request.ExcludeId);
        }

        return await query.AnyAsync(x => x.MenuName == request.MenuName, cancellationToken);
    }
}
