using Microsoft.EntityFrameworkCore;
using CleanAdmin.Domain.AggregatesModel.UserAggregate;

namespace CleanAdmin.ApiService.Application.Queries.Users;

public sealed record GetUserIdByNameQuery(string Username) : IQuery<UserId?>;

public sealed class GetUserIdByNameQueryHandler(
    ApplicationDbContext dbContext)
    : IQueryHandler<GetUserIdByNameQuery, UserId?>
{
    public async Task<UserId?> Handle(GetUserIdByNameQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .Where(u => u.Username == request.Username)
            .Select(u => u.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}