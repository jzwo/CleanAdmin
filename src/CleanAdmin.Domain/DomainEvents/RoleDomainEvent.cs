using CleanAdmin.Domain.AggregatesModel.RoleAggregate;

namespace CleanAdmin.Domain.DomainEvents;

public record RolePermissionChangedDomainEvent(Role Role) : IDomainEvent;

public record RoleDeletedDomainEvent(Role Role) : IDomainEvent;

public record RoleInfoChangedDomainEvent(Role Role) : IDomainEvent;

public record RoleMenusChangedDomainEvent(Role Role) : IDomainEvent;

public record RolePermissionsChangedDomainEvent(Role Role) : IDomainEvent;