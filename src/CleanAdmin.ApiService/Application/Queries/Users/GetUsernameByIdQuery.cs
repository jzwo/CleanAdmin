using Microsoft.EntityFrameworkCore;
using CleanAdmin.Domain.AggregatesModel.UserAggregate;

namespace CleanAdmin.ApiService.Application.Queries.Users;

public record GetUsernameByIdQuery(UserId UserId) : IQuery<string>;

public class GetUsernameByIdQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetUsernameByIdQuery, string>
{
    public async Task<string> Handle(GetUsernameByIdQuery request, CancellationToken cancellationToken)
    {
    return await context.Users
                   .Where(u => u.Id == request.UserId)
                   .Select(u => u.Username ?? string.Empty)
                   .FirstOrDefaultAsync(cancellationToken)
               ?? throw new KnownException("User not found");
    }
}