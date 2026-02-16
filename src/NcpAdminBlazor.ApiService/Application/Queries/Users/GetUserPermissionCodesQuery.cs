using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;

namespace NcpAdminBlazor.ApiService.Application.Queries.Users;

public record GetUserPermissionCodesQuery(UserId UserId) : IQuery<List<string>>;

public class GetUserPermissionCodesQueryValidator : AbstractValidator<GetUserPermissionCodesQuery>
{
    public GetUserPermissionCodesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("用户ID不能为空");
    }
}

public class GetUserPermissionCodesQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetUserPermissionCodesQuery, List<string>>
{
    public async Task<List<string>> Handle(GetUserPermissionCodesQuery request, CancellationToken cancellationToken)
    {
        var permissionCodes = await context.Users
            .AsNoTracking()
            .Where(u => u.Id == request.UserId && !u.IsDeleted)
            .Select(u => u.UserPermissions
                .Select(up => up.PermissionCode)
                .Distinct()
                .ToList())
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KnownException($"用户不存在，UserId = {request.UserId}");

        return permissionCodes;
    }
}
