using CleanAdmin.Domain.AggregatesModel.RoleAggregate;

namespace CleanAdmin.Infrastructure.Repositories;

public interface IRoleRepository : IRepository<Role, RoleId>;

public class RoleRepository(ApplicationDbContext context)
    : RepositoryBase<Role, RoleId, ApplicationDbContext>(context), IRoleRepository;