using CleanAdmin.Domain.AggregatesModel.MenuAggregate;
using CleanAdmin.Infrastructure.Repositories;

namespace CleanAdmin.ApiService.Application.Commands.Menus;

/// <summary>
/// 隐藏菜单命令
/// </summary>
public record HideMenuCommand(MenuId MenuId) : ICommand;

/// <summary>
/// 隐藏菜单命令验证器
/// </summary>
public class HideMenuCommandValidator : AbstractValidator<HideMenuCommand>
{
    public HideMenuCommandValidator()
    {
        RuleFor(x => x.MenuId)
            .NotEmpty()
            .WithMessage("菜单ID不能为空");
    }
}

/// <summary>
/// 隐藏菜单命令处理器
/// </summary>
public class HideMenuCommandHandler(IMenuRepository menuRepository)
    : ICommandHandler<HideMenuCommand>
{
    public async Task Handle(HideMenuCommand command, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.GetAsync(command.MenuId, cancellationToken)
            ?? throw new KnownException($"菜单不存在，MenuId = {command.MenuId}");

        menu.Hide();
    }
}
