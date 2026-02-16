using FastEndpoints;
using NcpAdminBlazor.ApiService.Application.Queries.Menus;
using NcpAdminBlazor.ApiService.Auth;

namespace NcpAdminBlazor.ApiService.Endpoints.Menus;

/// <summary>
/// 获取当前用户导航菜单端点
/// </summary>
public sealed class GetCurrentUserNavigationMenusEndpoint(IMediator mediator, ICurrentUser currentUser)
    : EndpointWithoutRequest<ResponseData<List<UserMenuTreeNodeDto>>>
{
    public override void Configure()
    {
        Get("/api/menus/current-user");
        Description(x => x.WithTags("Menus"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        _ = currentUser.UserId ?? throw new KnownException("未找到用户身份信息");
        var menus = await mediator.Send(new GetNavigationMenusByPermissionCodesQuery(currentUser.PermissionCodes.ToList()), ct);
        await Send.OkAsync(menus.AsResponseData(), ct);
    }
}

public sealed class GetCurrentUserNavigationMenusSummary : Summary<GetCurrentUserNavigationMenusEndpoint>
{
    public GetCurrentUserNavigationMenusSummary()
    {
        Summary = "获取当前用户导航菜单";
        Description = "根据当前登录用户的权限，返回可访问的导航菜单树";
    }
}


