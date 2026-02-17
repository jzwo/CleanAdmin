using CleanAdmin.Domain.AggregatesModel.DeliverAggregate;
using CleanAdmin.Domain.AggregatesModel.OrderAggregate;
using CleanAdmin.Infrastructure.Repositories;
using NetCorePal.Extensions.Primitives;

namespace CleanAdmin.ApiService.Application.Commands;

public record DeliverGoodsCommand(OrderId OrderId) : ICommand<DeliverRecordId>;

public class DeliverGoodsCommandHandler(IDeliverRecordRepository deliverRecordRepository)
    : ICommandHandler<DeliverGoodsCommand, DeliverRecordId>
{
    public Task<DeliverRecordId> Handle(DeliverGoodsCommand request, CancellationToken cancellationToken)
    {
        var record = new DeliverRecord(request.OrderId);
        deliverRecordRepository.Add(record);
        return Task.FromResult(record.Id);
    }
}