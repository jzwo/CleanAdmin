using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.ApiService.Application.Queries.MenusManagement;

namespace NcpAdminBlazor.ApiService.Endpoints.MenusManagement;

/// <summary>
/// 获取菜单信息端点
/// </summary>
public sealed class GetMenuInfoEndpoint(IMediator mediator)
    : Endpoint<GetMenuInfoRequest, ResponseData<MenuInfoDto>>
{
    public override void Configure()
    {
        Get("/api/menus/{MenuId}");
        Description(d => d.WithTags("Menu"));
        Summary(s =>
        {
            s.Summary = "获取菜单信息";
            s.Description = "根据菜单ID获取菜单详细信息";
        });
    }

    public override async Task HandleAsync(GetMenuInfoRequest req, CancellationToken ct)
    {
        var query = new GetMenuInfoQuery(req.MenuId);
        var result = await mediator.Send(query, ct);
        await Send.OkAsync(result.AsResponseData(), ct);
    }
}

/// <summary>
/// 获取菜单信息请求DTO
/// </summary>
public sealed class GetMenuInfoRequest
{
    public MenuId MenuId { get; init; } = default!;
}

/// <summary>
/// 获取菜单信息请求验证器
/// </summary>
public sealed class GetMenuInfoRequestValidator : AbstractValidator<GetMenuInfoRequest>
{
    public GetMenuInfoRequestValidator()
    {
        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage("菜单ID不能为空");
    }
}
