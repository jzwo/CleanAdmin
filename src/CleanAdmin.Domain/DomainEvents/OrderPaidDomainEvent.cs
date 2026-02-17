using CleanAdmin.Domain.AggregatesModel.OrderAggregate;

namespace CleanAdmin.Domain.DomainEvents;

public record OrderPaidDomainEvent(Order Order) : IDomainEvent;