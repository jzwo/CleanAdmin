using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.ApiService.Application.Commands.Menus;

/// <summary>
/// 显示菜单命令
/// </summary>
public record ShowMenuCommand(MenuId MenuId) : ICommand;

/// <summary>
/// 显示菜单命令验证器
/// </summary>
public class ShowMenuCommandValidator : AbstractValidator<ShowMenuCommand>
{
    public ShowMenuCommandValidator()
    {
        RuleFor(x => x.MenuId)
            .NotEmpty()
            .WithMessage("菜单ID不能为空");
    }
}

/// <summary>
/// 显示菜单命令处理器
/// </summary>
public class ShowMenuCommandHandler(IMenuRepository menuRepository)
    : ICommandHandler<ShowMenuCommand>
{
    public async Task Handle(ShowMenuCommand command, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.GetAsync(command.MenuId, cancellationToken)
            ?? throw new KnownException($"菜单不存在，MenuId = {command.MenuId}");

        menu.Show();
    }
}
