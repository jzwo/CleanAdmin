using CleanAdmin.ApiService.Auth.Permission;
using CleanAdmin.Domain.DomainEvents;

namespace CleanAdmin.ApiService.Application.DomainEventHandlers;

internal sealed class UserDeletedDomainEventHandlerForInvalidateUserPermissionCache(
    UserPermissionService userPermissionService)
    : IDomainEventHandler<UserDeletedDomainEvent>
{
    public Task Handle(UserDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        return userPermissionService.InvalidateAsync(notification.User.Id, cancellationToken).AsTask();
    }
}
