using CleanAdmin.Domain.AggregatesModel.RoleAggregate;
using CleanAdmin.Domain.AggregatesModel.UserAggregate;
using CleanAdmin.Infrastructure.Repositories;

namespace CleanAdmin.ApiService.Application.Commands.Users;

public sealed record RefreshUserPermissionsByRoleCommand(
    RoleId RoleId,
    List<string> PermissionCodes,
    List<UserId> AffectedUserIds) : ICommand;

public sealed class RefreshUserPermissionsByRoleCommandValidator : AbstractValidator<RefreshUserPermissionsByRoleCommand>
{
    public RefreshUserPermissionsByRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");

        RuleFor(x => x.PermissionCodes)
            .NotNull().WithMessage("权限代码列表不能为空");

        RuleFor(x => x.AffectedUserIds)
            .NotNull().WithMessage("受影响用户列表不能为空");
    }
}

internal sealed class RefreshUserPermissionsByRoleCommandHandler(
    IUserRepository userRepository)
    : ICommandHandler<RefreshUserPermissionsByRoleCommand>
{
    public async Task Handle(RefreshUserPermissionsByRoleCommand request, CancellationToken cancellationToken)
    {
        if (request.AffectedUserIds.Count == 0)
        {
            return;
        }

        foreach (var userId in request.AffectedUserIds.Distinct())
        {
            var user = await userRepository.GetAsync(userId, cancellationToken);
            if (user is null || user.IsDeleted)
            {
                continue;
            }

            user.UpdateRolePermissions(request.RoleId, request.PermissionCodes);
        }
    }
}
