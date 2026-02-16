using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Hybrid;
using NcpAdminBlazor.ApiService.Application.Queries.Users;
using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;
using NcpAdminBlazor.Shared.Auth;

namespace NcpAdminBlazor.ApiService.Auth.Permission;

internal sealed class UserPermissionHydrator(UserPermissionService userPermissionService) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        ArgumentNullException.ThrowIfNull(userId);
        var username = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        if (principal.Claims.Any(c => c.Type == "permissions"))
        {
            return principal;
        }

        if (username == SystemDefaultSuperAdmin.Username)
        {
            principal.AddIdentity(new ClaimsIdentity(AppPermissions.GetAllPermissionKeys()
                .Select(p => new Claim("permissions", p))));
            return principal;
        }

        // 从缓存读取所有权限代码加载到权限声明列表
        var userPermissions = await userPermissionService.GetPermissionsForUserAsync(userId);
        if (userPermissions.Length != 0)
            principal.AddIdentity(new ClaimsIdentity(userPermissions.Select(p => new Claim("permissions", p))));

        return principal;
    }
}

internal sealed class UserPermissionService(IMediator mediator, HybridCache hybridCache)
{
    private static readonly HybridCacheEntryOptions CacheOptions = new()
    {
        Expiration = TimeSpan.FromMinutes(10)
    };

    public async Task<string[]> GetPermissionsForUserAsync(string userId)
    {
        if (!UserId.TryParse(userId, out var parsedUserId))
        {
            return [];
        }

        return await hybridCache.GetOrCreateAsync(
            GetCacheKey(userId),
            async cancellationToken =>
            {
                var permissions = await mediator.Send(new GetUserPermissionCodesQuery(parsedUserId), cancellationToken);
                return permissions.ToArray();
            },
            CacheOptions);
    }

    public ValueTask InvalidateAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return hybridCache.RemoveAsync(GetCacheKey(userId.ToString()), cancellationToken);
    }

    private static string GetCacheKey(string userId)
    {
        return $"auth:permissions:{userId}";
    }
}