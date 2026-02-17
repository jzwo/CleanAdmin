using CleanAdmin.Domain.AggregatesModel.UserAggregate;

namespace CleanAdmin.Domain.DomainEvents;

public record UserCreatedDomainEvent(User User) : IDomainEvent;

public record UserInfoUpdatedDomainEvent(User User) : IDomainEvent;

public record UserLoginDomainEvent(User User) : IDomainEvent;

public record UserPasswordChangedDomainEvent(User User) : IDomainEvent;

public record UserDeletedDomainEvent(User User) : IDomainEvent;