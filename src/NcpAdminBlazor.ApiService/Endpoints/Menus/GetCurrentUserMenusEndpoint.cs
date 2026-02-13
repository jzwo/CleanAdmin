using FastEndpoints;
using NcpAdminBlazor.ApiService.Application.Queries.Menus;
using NcpAdminBlazor.ApiService.Auth;

namespace NcpAdminBlazor.ApiService.Endpoints.Menus;

/// <summary>
/// 获取当前用户菜单端点
/// </summary>
public sealed class GetCurrentUserMenusEndpoint(IMediator mediator, ICurrentUser currentUser) 
    : EndpointWithoutRequest<ResponseData<List<UserMenuTreeNodeDto>>>
{
    public override void Configure()
    {
        Get("/api/menus/current-user");
        Description(x => x.WithTags("Menus"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new KnownException("未找到用户身份信息");
        
        var menus = await mediator.Send(new GetCurrentUserMenusQuery(userId), ct);
        await Send.OkAsync(menus.AsResponseData(), ct);
    }
}

public sealed class GetCurrentUserMenusSummary : Summary<GetCurrentUserMenusEndpoint>
{
    public GetCurrentUserMenusSummary()
    {
        Summary = "获取当前用户菜单";
        Description = "根据当前登录用户的权限，返回可访问的菜单树";
    }
}


