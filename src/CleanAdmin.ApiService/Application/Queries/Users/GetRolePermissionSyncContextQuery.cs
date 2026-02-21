using Microsoft.EntityFrameworkCore;
using CleanAdmin.Domain.AggregatesModel.RoleAggregate;
using CleanAdmin.Domain.AggregatesModel.UserAggregate;

namespace CleanAdmin.ApiService.Application.Queries.Users;

public sealed record GetRolePermissionSyncContextQuery(RoleId RoleId) : IQuery<RolePermissionSyncContextDto>;

public sealed record RolePermissionSyncContextDto(
    RoleId RoleId,
    List<string> PermissionCodes,
    List<UserId> AffectedUserIds);

public sealed class GetRolePermissionSyncContextQueryValidator : AbstractValidator<GetRolePermissionSyncContextQuery>
{
    public GetRolePermissionSyncContextQueryValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");
    }
}

public sealed class GetRolePermissionSyncContextQueryHandler(ApplicationDbContext dbContext)
    : IQueryHandler<GetRolePermissionSyncContextQuery, RolePermissionSyncContextDto>
{
    public async Task<RolePermissionSyncContextDto> Handle(GetRolePermissionSyncContextQuery request,
        CancellationToken cancellationToken)
    {
        var permissionCodes = await dbContext.Roles
            .Where(role => role.Id == request.RoleId && !role.IsDeleted)
            .Select(role => role.AssignedPermissionCodes.ToList())
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KnownException($"未找到角色，RoleId = {request.RoleId}");

        var affectedUserIds = await dbContext.Users
            .Where(user => !user.IsDeleted && user.UserRoles.Any(userRole => userRole.RoleId == request.RoleId))
            .Select(user => user.Id)
            .ToListAsync(cancellationToken);

        return new RolePermissionSyncContextDto(request.RoleId, permissionCodes, affectedUserIds);
    }
}
