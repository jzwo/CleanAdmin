using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.ApiService.Application.Commands.Menus;

namespace NcpAdminBlazor.ApiService.Endpoints.MenusManagement;

/// <summary>
/// 删除菜单端点
/// </summary>
public sealed class DeleteMenuEndpoint(IMediator mediator)
    : Endpoint<DeleteMenuRequest, ResponseData<EmptyResponse>>
{
    public override void Configure()
    {
        Delete("/api/menus/{MenuId}");
        Description(d => d.WithTags("Menu"));
        Summary(s =>
        {
            s.Summary = "删除菜单";
            s.Description = "删除一个菜单项（菜单下无子菜单时才能删除）";
        });
    }

    public override async Task HandleAsync(DeleteMenuRequest req, CancellationToken ct)
    {
        var command = new DeleteMenuCommand(req.MenuId);
        await mediator.Send(command, ct);
        await Send.OkAsync(new EmptyResponse().AsResponseData(), ct);
    }
}

/// <summary>
/// 删除菜单请求DTO
/// </summary>
public sealed class DeleteMenuRequest
{
    public MenuId MenuId { get; init; } = default!;
}

/// <summary>
/// 删除菜单请求验证器
/// </summary>
public sealed class DeleteMenuRequestValidator : AbstractValidator<DeleteMenuRequest>
{
    public DeleteMenuRequestValidator()
    {
        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage("菜单ID不能为空");
    }
}
