using FastEndpoints;
using CleanAdmin.ApiService.Auth;

namespace CleanAdmin.ApiService.Endpoints.Users;

public sealed class CurrentUserPermissionCodesEndpoint(ICurrentUser currentUser)
    : EndpointWithoutRequest<ResponseData<List<string>>>
{
    public override void Configure()
    {
        Get("/api/user/permissions/current");
        Description(x => x.WithTags("User"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        _ = currentUser.UserId ?? throw new KnownException("not logged in");

        var permissionCodes = currentUser.PermissionCodes
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        await Send.OkAsync(permissionCodes.AsResponseData(), ct);
    }
}

public sealed class CurrentUserPermissionCodesSummary : Summary<CurrentUserPermissionCodesEndpoint>
{
    public CurrentUserPermissionCodesSummary()
    {
        Summary = "获取当前用户权限点";
        Description = "返回当前登录用户拥有的权限点编码列表";
    }
}
