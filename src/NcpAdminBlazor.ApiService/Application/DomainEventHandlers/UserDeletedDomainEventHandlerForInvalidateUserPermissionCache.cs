using NcpAdminBlazor.ApiService.Auth.Permission;
using NcpAdminBlazor.Domain.DomainEvents;

namespace NcpAdminBlazor.ApiService.Application.DomainEventHandlers;

internal sealed class UserDeletedDomainEventHandlerForInvalidateUserPermissionCache(
    UserPermissionService userPermissionService)
    : IDomainEventHandler<UserDeletedDomainEvent>
{
    public Task Handle(UserDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        return userPermissionService.InvalidateAsync(notification.User.Id, cancellationToken).AsTask();
    }
}
