using CleanAdmin.Domain.AggregatesModel.OrderAggregate;

namespace CleanAdmin.Domain.AggregatesModel.DeliverAggregate;

public partial record DeliverRecordId : IGuidStronglyTypedId ;

public class DeliverRecord : Entity<DeliverRecordId>, IAggregateRoot
{
    protected DeliverRecord()
    {
    }


    public DeliverRecord(OrderId orderId)
    {
        OrderId = orderId;
    }

    public OrderId OrderId { get; private set; } = null!;
}