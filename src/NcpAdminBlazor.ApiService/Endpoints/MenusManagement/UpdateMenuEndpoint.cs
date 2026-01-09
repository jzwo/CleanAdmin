using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.ApiService.Application.Commands.Menus;

namespace NcpAdminBlazor.ApiService.Endpoints.MenusManagement;

/// <summary>
/// 更新菜单端点
/// </summary>
public sealed class UpdateMenuEndpoint(IMediator mediator)
    : Endpoint<UpdateMenuRequest, ResponseData<EmptyResponse>>
{
    public override void Configure()
    {
        Put("/api/menus/{MenuId}");
        Description(d => d.WithTags("Menu"));
        Summary(s =>
        {
            s.Summary = "更新菜单";
            s.Description = "更新菜单的基本信息";
        });
    }

    public override async Task HandleAsync(UpdateMenuRequest req, CancellationToken ct)
    {
        var command = new UpdateMenuCommand(
            req.MenuId,
            req.MenuName,
            req.RoutePath,
            req.ComponentPath,
            req.Icon,
            req.PermissionCode);

        await mediator.Send(command, ct);
        await Send.OkAsync(new EmptyResponse().AsResponseData(), ct);
    }
}

/// <summary>
/// 更新菜单请求DTO
/// </summary>
public sealed class UpdateMenuRequest
{
    public MenuId MenuId { get; init; } = default!;
    public string MenuName { get; init; } = string.Empty;
    public string RoutePath { get; init; } = string.Empty;
    public string? ComponentPath { get; init; }
    public string? Icon { get; init; }
    public string? PermissionCode { get; init; }
}

/// <summary>
/// 更新菜单请求验证器
/// </summary>
public sealed class UpdateMenuRequestValidator : AbstractValidator<UpdateMenuRequest>
{
    public UpdateMenuRequestValidator()
    {
        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage("菜单ID不能为空");

        RuleFor(x => x.MenuName)
            .NotEmpty().WithMessage("菜单名称不能为空")
            .MaximumLength(100).WithMessage("菜单名称不能超过100个字符");

        RuleFor(x => x.RoutePath)
            .NotEmpty().WithMessage("路由路径不能为空")
            .MaximumLength(255).WithMessage("路由路径不能超过255个字符");

        RuleFor(x => x.ComponentPath)
            .MaximumLength(255).WithMessage("组件路径不能超过255个字符");

        RuleFor(x => x.Icon)
            .MaximumLength(100).WithMessage("图标不能超过100个字符");

        RuleFor(x => x.PermissionCode)
            .MaximumLength(100).WithMessage("权限码不能超过100个字符");
    }
}
