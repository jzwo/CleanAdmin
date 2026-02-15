using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;

namespace NcpAdminBlazor.ApiService.Application.Queries.Users;

public sealed record GetUserIdsByRoleIdQuery(RoleId RoleId) : IQuery<List<UserId>>;

public sealed class GetUserIdsByRoleIdQueryHandler(ApplicationDbContext dbContext)
    : IQueryHandler<GetUserIdsByRoleIdQuery, List<UserId>>
{
    public async Task<List<UserId>> Handle(GetUserIdsByRoleIdQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .Where(u => !u.IsDeleted && u.UserRoles.Any(ur => ur.RoleId == request.RoleId))
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);
    }
}