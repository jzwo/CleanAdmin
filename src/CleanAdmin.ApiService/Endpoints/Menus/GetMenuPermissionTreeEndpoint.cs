using FastEndpoints;
using CleanAdmin.ApiService.Application.Queries.Menus;

namespace CleanAdmin.ApiService.Endpoints.Menus;

/// <summary>
/// 获取菜单权限树（用于角色分配权限）
/// </summary>
public sealed class GetMenuPermissionTreeEndpoint(IMediator mediator)
    : EndpointWithoutRequest<ResponseData<List<MenuPermissionTreeNodeDto>>>
{
    public override void Configure()
    {
        Get("/api/menus/permissions/tree");
        Description(d => d.WithTags("Menu"));
        Summary(s =>
        {
            s.Summary = "获取菜单权限树";
            s.Description = "返回由菜单聚合组织的权限树，未绑定权限码的节点也会保留";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var tree = await mediator.Send(new GetMenuPermissionTreeQuery(), ct);
        await Send.OkAsync(tree.AsResponseData(), ct);
    }
}
