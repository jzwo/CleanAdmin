using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.Infrastructure.Repositories;

/// <summary>
/// 菜单仓储接口
/// </summary>
public interface IMenuRepository : IRepository<Menu, MenuId>
{
}

/// <summary>
/// 菜单仓储实现
/// </summary>
public class MenuRepository(ApplicationDbContext context) : RepositoryBase<Menu, MenuId, ApplicationDbContext>(context), IMenuRepository
{
}
