using CleanAdmin.ApiService.Auth.Permission;
using CleanAdmin.Domain.DomainEvents;

namespace CleanAdmin.ApiService.Application.DomainEventHandlers;

internal sealed class UserInfoUpdatedDomainEventHandlerForInvalidateUserPermissionCache(
    UserPermissionService userPermissionService)
    : IDomainEventHandler<UserInfoUpdatedDomainEvent>
{
    public Task Handle(UserInfoUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        return userPermissionService.InvalidateAsync(notification.User.Id, cancellationToken).AsTask();
    }
}
