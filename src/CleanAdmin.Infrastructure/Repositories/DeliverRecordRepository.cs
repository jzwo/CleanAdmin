using CleanAdmin.Domain.AggregatesModel.DeliverAggregate;

namespace CleanAdmin.Infrastructure.Repositories;

public interface IDeliverRecordRepository : IRepository<DeliverRecord, DeliverRecordId>;

public class DeliverRecordRepository(ApplicationDbContext context) : RepositoryBase<DeliverRecord, DeliverRecordId, ApplicationDbContext>(context), IDeliverRecordRepository;

