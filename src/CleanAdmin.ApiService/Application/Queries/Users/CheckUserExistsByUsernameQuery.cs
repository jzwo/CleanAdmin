using Microsoft.EntityFrameworkCore;

namespace CleanAdmin.ApiService.Application.Queries.Users;

public record CheckUserExistsByUsernameQuery(string Username) : IQuery<bool>;

public class CheckUserExistsByUsernameQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckUserExistsByUsernameQuery, bool>
{
    public async Task<bool> Handle(CheckUserExistsByUsernameQuery request, CancellationToken cancellationToken)
    {
        return await context.Users
            .AnyAsync(u => u.Username == request.Username, cancellationToken);
    }
}