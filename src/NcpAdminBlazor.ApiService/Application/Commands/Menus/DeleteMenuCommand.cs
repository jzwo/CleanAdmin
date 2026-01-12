using NcpAdminBlazor.ApiService.Application.Queries.Menus;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.ApiService.Application.Commands.Menus;

/// <summary>
/// 删除菜单命令
/// </summary>
public record DeleteMenuCommand(MenuId MenuId) : ICommand;

/// <summary>
/// 删除菜单命令验证器
/// </summary>
public class DeleteMenuCommandValidator : AbstractValidator<DeleteMenuCommand>
{
    private readonly IMediator _mediator;

    public DeleteMenuCommandValidator(IMediator mediator)
    {
        _mediator = mediator;

        RuleFor(x => x.MenuId)
            .NotEmpty()
            .WithMessage("菜单ID不能为空");

        // 异步验证
        RuleFor(x => x)
            .MustAsync(ValidateMenuAsync)
            .WithMessage("菜单验证失败");
    }

    private async Task<bool> ValidateMenuAsync(DeleteMenuCommand command, CancellationToken cancellationToken)
    {
        // 检查菜单是否有子菜单
        var hasChildren = await _mediator.Send(
            new CheckMenuHasChildrenQuery(command.MenuId),
            cancellationToken);
        if (hasChildren)
            throw new KnownException("菜单有子菜单，无法删除，请先删除子菜单");

        return true;
    }
}

/// <summary>
/// 删除菜单命令处理器
/// </summary>
public class DeleteMenuCommandHandler(IMenuRepository menuRepository)
    : ICommandHandler<DeleteMenuCommand>
{
    public async Task Handle(DeleteMenuCommand command, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.GetAsync(command.MenuId, cancellationToken)
            ?? throw new KnownException($"菜单不存在，MenuId = {command.MenuId}");

        menu.Delete();
    }
}
