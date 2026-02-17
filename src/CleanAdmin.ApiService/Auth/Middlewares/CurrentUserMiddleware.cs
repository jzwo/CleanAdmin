using System.Security.Claims;
using CleanAdmin.Domain.AggregatesModel.UserAggregate;

namespace CleanAdmin.ApiService.Auth.Middlewares;

/// <summary>
/// 中间件：从 ClaimsPrincipal 提取用户信息并填充到 ICurrentUser
/// </summary>
public class CurrentUserMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICurrentUser currentUser)
    {
        var claimsPrincipal = context.User;

        if (currentUser is not CurrentUser concreteCurrentUser)
        {
            await next(context);
            return;
        }

        if (claimsPrincipal.Identity is not { IsAuthenticated: true })
        {
            concreteCurrentUser.UserId = null;
            concreteCurrentUser.UserName = string.Empty;
            concreteCurrentUser.PermissionCodes = [];
            await next(context);
            return;
        }

        var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
        var userNameClaim = claimsPrincipal.FindFirst(ClaimTypes.Name);

        if (userIdClaim != null && UserId.TryParse(userIdClaim.Value, out var userId))
        {
            concreteCurrentUser.UserId = userId;
        }
        else
        {
            concreteCurrentUser.UserId = null;
        }

        concreteCurrentUser.UserName = userNameClaim != null ? userNameClaim.Value : string.Empty;
        concreteCurrentUser.PermissionCodes = claimsPrincipal
            .FindAll("permissions")
            .Select(c => c.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        await next(context);
    }
}
