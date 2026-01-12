using NcpAdminBlazor.ApiService.Application.Queries.Menus;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.ApiService.Application.Commands.Menus;

/// <summary>
/// 更新菜单命令
/// </summary>
public record UpdateMenuCommand(
    MenuId MenuId,
    string MenuName,
    string RoutePath,
    string? ComponentPath,
    string? Icon,
    string? PermissionCode) : ICommand;

/// <summary>
/// 更新菜单命令验证器
/// </summary>
public class UpdateMenuCommandValidator : AbstractValidator<UpdateMenuCommand>
{
    private readonly IMediator _mediator;

    public UpdateMenuCommandValidator(IMediator mediator)
    {
        _mediator = mediator;

        RuleFor(x => x.MenuId)
            .NotEmpty()
            .WithMessage("菜单ID不能为空");

        RuleFor(x => x.MenuName)
            .NotEmpty()
            .WithMessage("菜单名称不能为空")
            .MaximumLength(100)
            .WithMessage("菜单名称不能超过100个字符");

        RuleFor(x => x.RoutePath)
            .NotEmpty()
            .WithMessage("路由路径不能为空")
            .MaximumLength(255)
            .WithMessage("路由路径不能超过255个字符");

        RuleFor(x => x.ComponentPath)
            .MaximumLength(255)
            .WithMessage("组件路径不能超过255个字符");

        RuleFor(x => x.Icon)
            .MaximumLength(100)
            .WithMessage("图标不能超过100个字符");

        RuleFor(x => x.PermissionCode)
            .MaximumLength(100)
            .WithMessage("权限码不能超过100个字符");

        // 异步验证
        RuleFor(x => x)
            .MustAsync(ValidateMenuAsync)
            .WithMessage("菜单验证失败");
    }

    private async Task<bool> ValidateMenuAsync(UpdateMenuCommand command, CancellationToken cancellationToken)
    {
        // 检查路由路径是否唯一（排除自己）
        var routePathExists = await _mediator.Send(
            new CheckMenuRoutePathUniqueQuery(command.RoutePath, command.MenuId),
            cancellationToken);
        if (routePathExists)
            throw new KnownException($"路由路径已存在：{command.RoutePath}");

        return true;
    }
}

/// <summary>
/// 更新菜单命令处理器
/// </summary>
public class UpdateMenuCommandHandler(IMenuRepository menuRepository)
    : ICommandHandler<UpdateMenuCommand>
{
    public async Task Handle(UpdateMenuCommand command, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.GetAsync(command.MenuId, cancellationToken)
            ?? throw new KnownException($"菜单不存在，MenuId = {command.MenuId}");

        menu.Update(
            command.MenuName,
            command.RoutePath,
            command.ComponentPath,
            command.Icon,
            command.PermissionCode);
    }
}
