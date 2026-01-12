using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.ApiService.Application.Queries.Menus;

/// <summary>
/// 获取菜单列表查询
/// </summary>
public record GetMenuListQuery(
    string? MenuName = null,
    MenuType? MenuType = null,
    MenuStatus? Status = null,
    bool? IsVisible = null,
    IPageRequest? PageRequest = null) :
    IQuery<PagedData<MenuListItemDto>>;

/// <summary>
/// 菜单列表项DTO
/// </summary>
public record MenuListItemDto(
    MenuId MenuId,
    string MenuName,
    MenuType MenuType,
    MenuId? ParentId,
    string RoutePath,
    int SortOrder,
    bool IsVisible,
    MenuStatus Status,
    DateTimeOffset CreatedAt);

/// <summary>
/// 获取菜单列表查询验证器
/// </summary>
public class GetMenuListQueryValidator : AbstractValidator<GetMenuListQuery>
{
    public GetMenuListQueryValidator()
    {
        When(x => x.PageRequest != null, () =>
        {
            RuleFor(x => x.PageRequest!.PageIndex)
                .GreaterThan(0)
                .WithMessage("页码必须大于0");

            RuleFor(x => x.PageRequest!.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("每页条数必须在1-100之间");
        });
    }
}

/// <summary>
/// 获取菜单列表查询处理器
/// </summary>
public class GetMenuListQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetMenuListQuery, PagedData<MenuListItemDto>>
{
    public async Task<PagedData<MenuListItemDto>> Handle(GetMenuListQuery request, CancellationToken cancellationToken)
    {
        var queryable = context.Menus
            .Where(x => !x.IsDeleted)
            .WhereIf(!string.IsNullOrEmpty(request.MenuName), x => x.MenuName.Contains(request.MenuName!))
            .WhereIf(request.MenuType.HasValue, x => x.MenuType == request.MenuType)
            .WhereIf(request.Status.HasValue, x => x.Status == request.Status)
            .WhereIf(request.IsVisible.HasValue, x => x.IsVisible == request.IsVisible)
            .OrderBy(x => x.ParentId)
            .ThenBy(x => x.SortOrder)
            .Select(x => new MenuListItemDto(
                x.Id,
                x.MenuName,
                x.MenuType,
                x.ParentId,
                x.RoutePath,
                x.SortOrder,
                x.IsVisible,
                x.Status,
                x.CreatedAt));

        // 如果提供了分页参数则使用分页，否则返回所有数据
        if (request.PageRequest != null)
        {
            var result = await queryable.ToPagedDataAsync(request.PageRequest, cancellationToken);
            return result;
        }
        else
        {
            var items = await queryable.ToListAsync(cancellationToken);
            return new PagedData<MenuListItemDto>(items, items.Count, 1, 0);
        }
    }
}
