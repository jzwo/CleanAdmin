using CleanAdmin.ApiService.Application.Commands.Users;
using CleanAdmin.ApiService.Application.Queries.Users;
using CleanAdmin.ApiService.Auth.Permission;
using CleanAdmin.Domain.DomainEvents;

namespace CleanAdmin.ApiService.Application.DomainEventHandlers;

internal sealed class RolePermissionsChangedDomainEventHandlerForRefreshUsersPermissions(
    IMediator mediator,
    UserPermissionService userPermissionService)
    : IDomainEventHandler<RolePermissionsChangedDomainEvent>
{
    public async Task Handle(RolePermissionsChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var syncContext = await mediator.Send(
            new GetRolePermissionSyncContextQuery(notification.Role.Id),
            cancellationToken);

        await mediator.Send(
            new RefreshUserPermissionsByRoleCommand(
                syncContext.RoleId,
                syncContext.PermissionCodes,
                syncContext.AffectedUserIds),
            cancellationToken);

        foreach (var userId in syncContext.AffectedUserIds)
        {
            await userPermissionService.InvalidateAsync(userId, cancellationToken);
        }
    }
}
