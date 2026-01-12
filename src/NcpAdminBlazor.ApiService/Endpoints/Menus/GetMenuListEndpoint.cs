using FastEndpoints;
using NcpAdminBlazor.ApiService.Application.Queries.Menus;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.ApiService.Endpoints.Menus;

/// <summary>
/// 获取菜单列表端点
/// </summary>
public sealed class GetMenuListEndpoint(IMediator mediator)
    : Endpoint<GetMenuListRequest, ResponseData<PagedData<MenuListItemDto>>>
{
    public override void Configure()
    {
        Get("/api/menus");
        Description(d => d.WithTags("Menu"));
        Summary(s =>
        {
            s.Summary = "获取菜单列表";
            s.Description = "分页获取菜单列表，支持按菜单名称、类型、状态、可见性筛选";
        });
    }

    public override async Task HandleAsync(GetMenuListRequest req, CancellationToken ct)
    {
        var query = new GetMenuListQuery(
            req.MenuName,
            req.MenuType,
            req.Status,
            req.IsVisible,
            req);

        var result = await mediator.Send(query, ct);
        await Send.OkAsync(result.AsResponseData(), ct);
    }
}

/// <summary>
/// 获取菜单列表请求DTO
/// </summary>
public sealed class GetMenuListRequest : PageRequest
{
    public string? MenuName { get; init; }
    public MenuType? MenuType { get; init; }
    public MenuStatus? Status { get; init; }
    public bool? IsVisible { get; init; }
}

/// <summary>
/// 获取菜单列表请求验证器
/// </summary>
public sealed class GetMenuListRequestValidator : AbstractValidator<GetMenuListRequest>
{
    public GetMenuListRequestValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThan(0).WithMessage("页码必须大于0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("每页条数必须在1-100之间");
    }
}