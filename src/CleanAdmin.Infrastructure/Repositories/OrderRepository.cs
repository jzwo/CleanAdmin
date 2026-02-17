using CleanAdmin.Domain.AggregatesModel.OrderAggregate;

namespace CleanAdmin.Infrastructure.Repositories;

public interface IOrderRepository : IRepository<Order, OrderId>;

public class OrderRepository(ApplicationDbContext context)
    : RepositoryBase<Order, OrderId, ApplicationDbContext>(context), IOrderRepository;