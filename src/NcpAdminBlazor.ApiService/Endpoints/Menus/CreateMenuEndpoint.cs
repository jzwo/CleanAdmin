using FastEndpoints;
using NcpAdminBlazor.ApiService.Application.Commands.Menus;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.ApiService.Endpoints.Menus;

/// <summary>
/// 创建菜单端点
/// </summary>
public sealed class CreateMenuEndpoint(IMediator mediator)
    : Endpoint<CreateMenuRequest, ResponseData<CreateMenuResponse>>
{
    public override void Configure()
    {
        Post("/api/menus");
        Description(d => d.WithTags("Menu"));
        Summary(s =>
        {
            s.Summary = "创建菜单";
            s.Description = "创建一个新的菜单项";
        });
    }

    public override async Task HandleAsync(CreateMenuRequest req, CancellationToken ct)
    {
        var command = new CreateMenuCommand(
            req.MenuName,
            req.MenuType,
            req.ParentId,
            req.RoutePath,
            req.ComponentPath,
            req.Icon,
            req.SortOrder,
            req.PermissionCode);

        var menuId = await mediator.Send(command, ct);
        await Send.OkAsync(new CreateMenuResponse(menuId).AsResponseData(), ct);
    }
}

/// <summary>
/// 创建菜单请求DTO
/// </summary>
public sealed class CreateMenuRequest
{
    public string MenuName { get; init; } = string.Empty;
    public MenuType MenuType { get; init; }
    public MenuId? ParentId { get; init; }
    public string RoutePath { get; init; } = string.Empty;
    public string? ComponentPath { get; init; }
    public string? Icon { get; init; }
    public int SortOrder { get; init; }
    public string? PermissionCode { get; init; }
}

/// <summary>
/// 创建菜单响应DTO
/// </summary>
public sealed record CreateMenuResponse(MenuId MenuId);

/// <summary>
/// 创建菜单请求验证器
/// </summary>
public sealed class CreateMenuRequestValidator : AbstractValidator<CreateMenuRequest>
{
    public CreateMenuRequestValidator()
    {
        RuleFor(x => x.MenuName)
            .NotEmpty().WithMessage("菜单名称不能为空")
            .MaximumLength(100).WithMessage("菜单名称不能超过100个字符");

        RuleFor(x => x.MenuType)
            .IsInEnum().WithMessage("菜单类型值无效");

        RuleFor(x => x.RoutePath)
            .NotEmpty().WithMessage("路由路径不能为空")
            .MaximumLength(255).WithMessage("路由路径不能超过255个字符");

        RuleFor(x => x.ComponentPath)
            .MaximumLength(255).WithMessage("组件路径不能超过255个字符");

        RuleFor(x => x.Icon)
            .MaximumLength(100).WithMessage("图标不能超过100个字符");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0).WithMessage("排序号不能为负数");

        RuleFor(x => x.PermissionCode)
            .MaximumLength(100).WithMessage("权限码不能超过100个字符");
    }
}
