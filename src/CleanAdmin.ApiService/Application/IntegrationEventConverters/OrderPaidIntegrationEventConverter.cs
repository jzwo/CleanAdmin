using CleanAdmin.Domain.DomainEvents;
using CleanAdmin.ApiService.Application.IntegrationEventHandlers;
using NetCorePal.Extensions.DistributedTransactions;

namespace CleanAdmin.ApiService.Application.IntegrationEventConverters;

public class OrderPaidIntegrationEventConverter
    : IIntegrationEventConverter<OrderPaidDomainEvent, OrderPaidIntegrationEvent>
{
    public OrderPaidIntegrationEvent Convert(OrderPaidDomainEvent domainEvent)
    {
        return new OrderPaidIntegrationEvent(domainEvent.Order.Id);
    }
}