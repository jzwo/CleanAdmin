using NcpAdminBlazor.ApiService.Auth.Permission;
using NcpAdminBlazor.Domain.DomainEvents;

namespace NcpAdminBlazor.ApiService.Application.DomainEventHandlers;

internal sealed class UserInfoUpdatedDomainEventHandlerForInvalidateUserPermissionCache(
    UserPermissionService userPermissionService)
    : IDomainEventHandler<UserInfoUpdatedDomainEvent>
{
    public Task Handle(UserInfoUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        return userPermissionService.InvalidateAsync(notification.User.Id, cancellationToken).AsTask();
    }
}
