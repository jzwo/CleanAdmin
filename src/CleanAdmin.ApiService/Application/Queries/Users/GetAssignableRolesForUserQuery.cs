using Microsoft.EntityFrameworkCore;
using CleanAdmin.Domain.AggregatesModel.RoleAggregate;

namespace CleanAdmin.ApiService.Application.Queries.Users;

public sealed record GetAssignableRolesForUserQuery : IQuery<List<AssignableRoleItemDto>>;

public sealed record AssignableRoleItemDto(
    RoleId RoleId,
    string RoleName);

public sealed class GetAssignableRolesForUserQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetAssignableRolesForUserQuery, List<AssignableRoleItemDto>>
{
    public async Task<List<AssignableRoleItemDto>> Handle(GetAssignableRolesForUserQuery request,
        CancellationToken cancellationToken)
    {
        return await context.Roles
            .Where(r => !r.IsDisabled)
            .OrderBy(r => r.Name)
            .Select(r => new AssignableRoleItemDto(r.Id, r.Name))
            .ToListAsync(cancellationToken);
    }
}