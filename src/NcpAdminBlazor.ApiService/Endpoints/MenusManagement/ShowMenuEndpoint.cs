using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.ApiService.Application.Commands.Menus;

namespace NcpAdminBlazor.ApiService.Endpoints.MenusManagement;

/// <summary>
/// 显示菜单端点
/// </summary>
public sealed class ShowMenuEndpoint(IMediator mediator)
    : Endpoint<ShowMenuRequest, ResponseData<EmptyResponse>>
{
    public override void Configure()
    {
        Patch("/api/menus/{MenuId}/show");
        Description(d => d.WithTags("Menu"));
        Summary(s =>
        {
            s.Summary = "显示菜单";
            s.Description = "显示一个隐藏的菜单";
        });
    }

    public override async Task HandleAsync(ShowMenuRequest req, CancellationToken ct)
    {
        var command = new ShowMenuCommand(req.MenuId);
        await mediator.Send(command, ct);
        await Send.OkAsync(new EmptyResponse().AsResponseData(), ct);
    }
}

/// <summary>
/// 显示菜单请求DTO
/// </summary>
public sealed class ShowMenuRequest
{
    public MenuId MenuId { get; init; } = default!;
}

/// <summary>
/// 显示菜单请求验证器
/// </summary>
public sealed class ShowMenuRequestValidator : AbstractValidator<ShowMenuRequest>
{
    public ShowMenuRequestValidator()
    {
        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage("菜单ID不能为空");
    }
}
