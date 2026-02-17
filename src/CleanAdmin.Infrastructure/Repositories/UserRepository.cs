using CleanAdmin.Domain.AggregatesModel.UserAggregate;

namespace CleanAdmin.Infrastructure.Repositories;

public interface IUserRepository : IRepository<User, UserId>;

public class UserRepository(ApplicationDbContext context)
    : RepositoryBase<User, UserId, ApplicationDbContext>(context), IUserRepository;