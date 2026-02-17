using CleanAdmin.Domain.AggregatesModel.MenuAggregate;
using CleanAdmin.Infrastructure.Repositories;

namespace CleanAdmin.ApiService.Application.Commands.Menus;

/// <summary>
/// 更新菜单排序号命令
/// </summary>
public record UpdateMenuSortOrderCommand(MenuId MenuId, int NewSortOrder) : ICommand;

/// <summary>
/// 更新菜单排序号命令验证器
/// </summary>
public class UpdateMenuSortOrderCommandValidator : AbstractValidator<UpdateMenuSortOrderCommand>
{
    private readonly IMediator _mediator;

    public UpdateMenuSortOrderCommandValidator(IMediator mediator)
    {
        _mediator = mediator;

        RuleFor(x => x.MenuId)
            .NotEmpty()
            .WithMessage("菜单ID不能为空");

        RuleFor(x => x.NewSortOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("排序号不能为负数");
    }
}

/// <summary>
/// 更新菜单排序号命令处理器
/// </summary>
public class UpdateMenuSortOrderCommandHandler(IMenuRepository menuRepository)
    : ICommandHandler<UpdateMenuSortOrderCommand>
{
    public async Task Handle(UpdateMenuSortOrderCommand command, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.GetAsync(command.MenuId, cancellationToken)
            ?? throw new KnownException($"菜单不存在，MenuId = {command.MenuId}");

        menu.UpdateSortOrder(command.NewSortOrder);
    }
}
