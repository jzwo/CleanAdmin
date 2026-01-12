using NcpAdminBlazor.ApiService.Application.Queries.Users;
using NcpAdminBlazor.Domain.DomainEvents;

namespace NcpAdminBlazor.ApiService.Application.DomainEventHandlers;

public class RoleDeletedDomainEventHandler(IMediator mediator) : IDomainEventHandler<RoleDeletedDomainEvent>
{
    public async Task Handle(RoleDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var role = notification.Role;
        var affectedUserIds = await mediator.Send(new GetUserIdsByRoleIdQuery(role.Id), cancellationToken);
        foreach (var userId in affectedUserIds)
        {
            // await mediator.Send(new RemoveUserRoleCommand(userId, role.Id), cancellationToken);
        }
    }
}