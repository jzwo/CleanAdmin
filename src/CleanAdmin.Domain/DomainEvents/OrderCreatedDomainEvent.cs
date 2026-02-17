using CleanAdmin.Domain.AggregatesModel.OrderAggregate;

namespace CleanAdmin.Domain.DomainEvents
{
    public record OrderCreatedDomainEvent(Order Order) : IDomainEvent;
}
