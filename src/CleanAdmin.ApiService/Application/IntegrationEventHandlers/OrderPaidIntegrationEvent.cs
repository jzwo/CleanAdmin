using CleanAdmin.Domain.AggregatesModel.OrderAggregate;

namespace CleanAdmin.ApiService.Application.IntegrationEventHandlers
{
    public record OrderPaidIntegrationEvent(OrderId OrderId);
}
