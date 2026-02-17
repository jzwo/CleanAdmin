using FastEndpoints;
using CleanAdmin.ApiService.Application.Commands.Menus;
using CleanAdmin.Domain.AggregatesModel.MenuAggregate;

namespace CleanAdmin.ApiService.Endpoints.Menus;

/// <summary>
/// 隐藏菜单端点
/// </summary>
public sealed class HideMenuEndpoint(IMediator mediator)
    : Endpoint<HideMenuRequest, ResponseData<EmptyResponse>>
{
    public override void Configure()
    {
        Patch("/api/menus/{MenuId}/hide");
        Description(d => d.WithTags("Menu"));
        Summary(s =>
        {
            s.Summary = "隐藏菜单";
            s.Description = "隐藏一个菜单项";
        });
    }

    public override async Task HandleAsync(HideMenuRequest req, CancellationToken ct)
    {
        var command = new HideMenuCommand(req.MenuId);
        await mediator.Send(command, ct);
        await Send.OkAsync(new EmptyResponse().AsResponseData(), ct);
    }
}

/// <summary>
/// 隐藏菜单请求DTO
/// </summary>
public sealed class HideMenuRequest
{
    public MenuId MenuId { get; init; } = default!;
}

/// <summary>
/// 隐藏菜单请求验证器
/// </summary>
public sealed class HideMenuRequestValidator : AbstractValidator<HideMenuRequest>
{
    public HideMenuRequestValidator()
    {
        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage("菜单ID不能为空");
    }
}
