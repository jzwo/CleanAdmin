using CleanAdmin.ApiService.Application.Queries.Menus;
using CleanAdmin.Domain.AggregatesModel.MenuAggregate;
using CleanAdmin.Infrastructure.Repositories;

namespace CleanAdmin.ApiService.Application.Commands.Menus;

/// <summary>
/// 创建菜单命令
/// </summary>
public record CreateMenuCommand(
    string MenuName,
    MenuType MenuType,
    MenuId? ParentId,
    string RoutePath,
    string? ComponentPath,
    string? Icon,
    int SortOrder,
    string? PermissionCode) : ICommand<MenuId>;

/// <summary>
/// 创建菜单命令验证器
/// </summary>
public class CreateMenuCommandValidator : AbstractValidator<CreateMenuCommand>
{
    private readonly IMediator _mediator;

    public CreateMenuCommandValidator(IMediator mediator)
    {
        _mediator = mediator;

        RuleFor(x => x.MenuName)
            .NotEmpty()
            .WithMessage("菜单名称不能为空")
            .MaximumLength(100)
            .WithMessage("菜单名称不能超过100个字符");

        RuleFor(x => x.MenuType)
            .IsInEnum()
            .WithMessage("菜单类型值无效");

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

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("排序号不能为负数");

        RuleFor(x => x.PermissionCode)
            .MaximumLength(100)
            .WithMessage("权限码不能超过100个字符");

        // 异步验证
        RuleFor(x => x)
            .MustAsync(ValidateMenuAsync)
            .WithMessage("菜单验证失败");
    }

    private async Task<bool> ValidateMenuAsync(CreateMenuCommand command, CancellationToken cancellationToken)
    {
        // 检查路由路径是否唯一
        var routePathExists = await _mediator.Send(
            new CheckMenuRoutePathUniqueQuery(command.RoutePath),
            cancellationToken);
        if (routePathExists)
            throw new KnownException($"路由路径已存在：{command.RoutePath}");

        // 检查父菜单是否存在
        if (command.ParentId is not null)
        {
            var parentExists = await _mediator.Send(
                new CheckParentMenuExistsQuery(command.ParentId),
                cancellationToken);
            if (!parentExists)
                throw new KnownException($"父菜单不存在，ParentId = {command.ParentId}");
        }

        // 检查同级菜单名称是否重复
        var nameExists = await _mediator.Send(
            new CheckMenuNameConflictQuery(command.MenuName, command.ParentId),
            cancellationToken);
        if (nameExists)
            throw new KnownException("同级菜单中已存在相同名称");

        // 检查同级菜单排序号是否重复
        var sortOrderExists = await _mediator.Send(
            new CheckMenuSortOrderConflictQuery(command.SortOrder, command.ParentId),
            cancellationToken);
        if (sortOrderExists)
            throw new KnownException("同级菜单中排序号已存在");

        return true;
    }
}

/// <summary>
/// 创建菜单命令处理器
/// </summary>
public class CreateMenuCommandHandler(IMenuRepository menuRepository)
    : ICommandHandler<CreateMenuCommand, MenuId>
{
    public async Task<MenuId> Handle(CreateMenuCommand command, CancellationToken cancellationToken)
    {
        // 创建菜单
        var menu = new Menu(
            command.MenuName,
            command.MenuType,
            command.ParentId,
            command.RoutePath,
            command.ComponentPath,
            command.Icon,
            command.SortOrder,
            command.PermissionCode);

        // 添加到仓储
        await menuRepository.AddAsync(menu, cancellationToken);

        return menu.Id;
    }
}