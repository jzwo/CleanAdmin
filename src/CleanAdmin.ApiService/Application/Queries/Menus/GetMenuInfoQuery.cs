using Microsoft.EntityFrameworkCore;
using CleanAdmin.Domain.AggregatesModel.MenuAggregate;

namespace CleanAdmin.ApiService.Application.Queries.Menus;

/// <summary>
/// 获取菜单信息查询
/// </summary>
public record GetMenuInfoQuery(MenuId MenuId) : IQuery<MenuInfoDto>;

/// <summary>
/// 菜单信息DTO
/// </summary>
public record MenuInfoDto(
    MenuId MenuId,
    string MenuName,
    MenuType MenuType,
    MenuId? ParentId,
    string RoutePath,
    string? ComponentPath,
    string? Icon,
    int SortOrder,
    bool IsExternal,
    bool IsVisible,
    string? PermissionCode,
    MenuStatus Status,
    DateTimeOffset CreatedAt);

/// <summary>
/// 获取菜单信息查询验证器
/// </summary>
public class GetMenuInfoQueryValidator : AbstractValidator<GetMenuInfoQuery>
{
    public GetMenuInfoQueryValidator()
    {
        RuleFor(x => x.MenuId)
            .NotEmpty()
            .WithMessage("菜单ID不能为空");
    }
}

/// <summary>
/// 获取菜单信息查询处理器
/// </summary>
public class GetMenuInfoQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetMenuInfoQuery, MenuInfoDto>
{
    public async Task<MenuInfoDto> Handle(GetMenuInfoQuery request, CancellationToken cancellationToken)
    {
        var menu = await context.Menus
            .Where(x => x.Id == request.MenuId && !x.IsDeleted)
            .Select(x => new MenuInfoDto(
                x.Id,
                x.MenuName,
                x.MenuType,
                x.ParentId,
                x.RoutePath,
                x.ComponentPath,
                x.Icon,
                x.SortOrder,
                x.IsExternal,
                x.IsVisible,
                x.PermissionCode,
                x.Status,
                x.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KnownException($"菜单不存在，MenuId = {request.MenuId}");

        return menu;
    }
}
