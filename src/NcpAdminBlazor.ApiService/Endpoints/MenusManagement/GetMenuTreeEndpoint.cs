using FastEndpoints;
using NcpAdminBlazor.ApiService.Application.Queries.MenusManagement;

namespace NcpAdminBlazor.ApiService.Endpoints.MenusManagement;

/// <summary>
/// 获取菜单树端点
/// </summary>
public sealed class GetMenuTreeEndpoint(IMediator mediator)
    : EndpointWithoutRequest<ResponseData<List<MenuTreeNodeDto>>>
{
    public override void Configure()
    {
        Get("/api/menus/tree/all");
        Description(d => d.WithTags("Menu"));
        Summary(s =>
        {
            s.Summary = "获取菜单树";
            s.Description = "获取完整的菜单树形结构（包含所有层级和子菜单）";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var query = new GetMenuTreeQuery();
        var result = await mediator.Send(query, ct);
        await Send.OkAsync(result.AsResponseData(), ct);
    }
}
