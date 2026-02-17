using Microsoft.EntityFrameworkCore;
using CleanAdmin.Domain.AggregatesModel.RoleAggregate;
using CleanAdmin.Domain.AggregatesModel.UserAggregate;

namespace CleanAdmin.ApiService.Application.Queries.Users;

public record GetUserInfoQuery(UserId UserId) : IQuery<UserInfoDto>;

public record UserInfoDto(
    UserId Id,
    string Username,
    string Email,
    string Phone,
    string RealName,
    DateTimeOffset CreatedAt,
    List<UserRoleSummaryDto> Roles
);

public record UserRoleSummaryDto(RoleId RoleId, string RoleName);

public class GetUserInfoQueryValidator : AbstractValidator<GetUserInfoQuery>
{
    public GetUserInfoQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("用户ID不能为空");
    }
}

public class GetUserInfoQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetUserInfoQuery, UserInfoDto>
{
    public async Task<UserInfoDto> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        var user = await context.Users
                       .Where(u => u.Id == request.UserId && !u.IsDeleted)
                       .Select(u => new
                       {
                           u.Id,
                           u.Username,
                           u.Email,
                           u.Phone,
                           u.RealName,
                           u.CreatedAt,
                           Roles = u.UserRoles.Select(ur => new UserRoleSummaryDto(ur.RoleId, ur.RoleName)).ToList()
                       })
                       .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new KnownException($"用户不存在，UserId = {request.UserId}");

        return new UserInfoDto(
            user.Id,
            user.Username,
            user.Email,
            user.Phone,
            user.RealName,
            user.CreatedAt,
            user.Roles);
    }
}