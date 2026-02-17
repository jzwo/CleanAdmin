using FastEndpoints;
using CleanAdmin.ApiService.Application.Commands.Menus;
using CleanAdmin.Domain.AggregatesModel.MenuAggregate;

namespace CleanAdmin.ApiService.Endpoints.Menus;

/// <summary>
/// 更新菜单排序号端点
/// </summary>
public sealed class UpdateMenuSortOrderEndpoint(IMediator mediator)
    : Endpoint<UpdateMenuSortOrderRequest, ResponseData<EmptyResponse>>
{
    public override void Configure()
    {
        Patch("/api/menus/{MenuId}/sort-order");
        Description(d => d.WithTags("Menu"));
        Summary(s =>
        {
            s.Summary = "更新菜单排序号";
            s.Description = "调整菜单的排序号";
        });
    }

    public override async Task HandleAsync(UpdateMenuSortOrderRequest req, CancellationToken ct)
    {
        var command = new UpdateMenuSortOrderCommand(req.MenuId, req.NewSortOrder);
        await mediator.Send(command, ct);
        await Send.OkAsync(new EmptyResponse().AsResponseData(), ct);
    }
}

/// <summary>
/// 更新菜单排序号请求DTO
/// </summary>
public sealed class UpdateMenuSortOrderRequest
{
    public MenuId MenuId { get; init; } = default!;
    public int NewSortOrder { get; init; }
}

/// <summary>
/// 更新菜单排序号请求验证器
/// </summary>
public sealed class UpdateMenuSortOrderRequestValidator : AbstractValidator<UpdateMenuSortOrderRequest>
{
    public UpdateMenuSortOrderRequestValidator()
    {
        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage("菜单ID不能为空");

        RuleFor(x => x.NewSortOrder)
            .GreaterThanOrEqualTo(0).WithMessage("排序号不能为负数");
    }
}
