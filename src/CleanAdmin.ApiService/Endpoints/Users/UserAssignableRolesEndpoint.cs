using FastEndpoints;
using CleanAdmin.ApiService.Application.Queries.Users;
using CleanAdmin.Domain.AggregatesModel.RoleAggregate;

namespace CleanAdmin.ApiService.Endpoints.Users;

public sealed class UserAssignableRolesEndpoint(IMediator mediator)
    : EndpointWithoutRequest<ResponseData<List<UserAssignableRoleItemResponse>>>
{
    public override void Configure()
    {
        Get("/api/user/roles/assignable");
        Description(d => d.WithTags("User"));
        Permissions(AppPermissions.System_Users_Update);
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
