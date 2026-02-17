using Microsoft.EntityFrameworkCore;
using CleanAdmin.Domain.AggregatesModel.UserAggregate;

namespace CleanAdmin.ApiService.Application.Queries.Users;

public record CheckUserExistsByUsernameExceptIdQuery(string Username, UserId UserId) : IQuery<bool>;

public class CheckUserExistsByUsernameExceptIdQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckUserExistsByUsernameExceptIdQuery, bool>
{
    public async Task<bool> Handle(CheckUserExistsByUsernameExceptIdQuery request, CancellationToken cancellationToken)
    {
        return await context.Users
            .AnyAsync(u => u.Username == request.Username && u.Id != request.UserId, cancellationToken);
    }
}