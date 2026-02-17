using FastEndpoints;
using NcpAdminBlazor.ApiService.Application.Queries.Users;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.ApiService.Endpoints.Users;

public sealed class UserAssignableRolesEndpoint(IMediator mediator)
    : EndpointWithoutRequest<ResponseData<List<UserAssignableRoleItemResponse>>>
{
    public override void Configure()
    {
        Get("/api/user/roles/assignable");
        Description(d => d.WithTags("User"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var roles = await mediator.Send(new GetAssignableRolesForUserQuery(), ct);
        var response = roles
            .Select(x => new UserAssignableRoleItemResponse(x.RoleId, x.RoleName))
            .ToList();

        await Send.OkAsync(response.AsResponseData(), ct);
    }
}

public sealed record UserAssignableRoleItemResponse(
    RoleId RoleId,
    string RoleName);

public sealed class UserAssignableRolesSummary : Summary<UserAssignableRolesEndpoint>
{
    public UserAssignableRolesSummary()
    {
        Summary = "获取可分配角色列表";
        Description = "返回用户分配角色场景可用的角色（仅启用角色）";
    }
}