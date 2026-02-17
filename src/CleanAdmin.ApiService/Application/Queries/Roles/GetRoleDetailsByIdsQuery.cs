using Microsoft.EntityFrameworkCore;
using CleanAdmin.Domain.AggregatesModel.RoleAggregate;

namespace CleanAdmin.ApiService.Application.Queries.Roles;

public record RoleDetailDto(RoleId RoleId, string RoleName, List<string> PermissionCodes);

public record GetRoleDetailsByIdsQuery(List<RoleId> RoleIds) : IQuery<List<RoleDetailDto>>;

public class GetRoleDetailsByIdsQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetRoleDetailsByIdsQuery, List<RoleDetailDto>>
{
    public async Task<List<RoleDetailDto>> Handle(GetRoleDetailsByIdsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.RoleIds.Count == 0) return [];
        var roles = await context.Roles
            .Where(r => request.RoleIds.Contains(r.Id) && !r.IsDeleted)
            .Select(r => new RoleDetailDto(r.Id, r.Name, r.AssignedPermissionCodes.ToList()))
            .ToListAsync(cancellationToken);

        return roles;
    }
}