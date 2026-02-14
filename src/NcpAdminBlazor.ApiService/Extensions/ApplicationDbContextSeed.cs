using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;
using NcpAdminBlazor.Infrastructure.Utils;
using NcpAdminBlazor.Shared.Auth;

namespace NcpAdminBlazor.ApiService.Extensions;

public static class ApplicationDbContextSeed
{
    public static async Task SeedAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(ApplicationDbContextSeed));
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        try
        {
            if (!await context.Roles.AnyAsync(cancellationToken))
            {
                var adminRole = new Role("Admin", "system admin role", false);
                adminRole.UpdatePermissions(AppPermissions.GetAllPermissionKeys().ToArray());
                await context.Roles.AddAsync(adminRole, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }

            if (!await context.Users.AnyAsync(cancellationToken))
            {
                var adminRoleId = await context.Roles
                    .Where(r => r.Name == "Admin")
                    .Select(r => r.Id)
                    .FirstAsync(cancellationToken);

                var adminUser = new User(
                    username: "admin",
                    passwordHash: passwordHasher.HashPassword("admin123456"),
                    realName: "system administrator",
                    email: "admin@example.com",
                    phone: "13800000000",
                    assignedRoleIds: new List<RoleId> { adminRoleId });

                await context.Users.AddAsync(adminUser, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }

            if (!await context.Menus.AnyAsync(cancellationToken))
            {
                // 创建主菜单
                var homeMenu = new Menu(
                    menuName: "首页",
                    menuType: MenuType.Menu,
                    parentId: null,
                    routePath: "/",
                    componentPath: null,
                    icon: "lucide:home",
                    sortOrder: 1,
                    permissionCode: null);

                var systemMenu = new Menu(
                    menuName: "系统管理",
                    menuType: MenuType.Directory,
                    parentId: null,
                    routePath: "/SystemManage",
                    componentPath: null,
                    icon: "lucide:settings",
                    sortOrder: 2,
                    permissionCode: null);

                var chatMenu = new Menu(
                    menuName: "AI Chat",
                    menuType: MenuType.Menu,
                    parentId: null,
                    routePath: "/chat",
                    componentPath: null,
                    icon: null,
                    sortOrder: 3,
                    permissionCode: null);

                await context.Menus.AddRangeAsync([homeMenu, systemMenu, chatMenu], cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                // 创建系统管理子菜单
                var userMenu = new Menu(
                    menuName: "用户管理",
                    menuType: MenuType.Menu,
                    parentId: systemMenu.Id,
                    routePath: "/SystemManage/User",
                    componentPath: null,
                    icon: "lucide:user-cog",
                    sortOrder: 1,
                    permissionCode: "");

                var roleMenu = new Menu(
                    menuName: "角色管理",
                    menuType: MenuType.Menu,
                    parentId: systemMenu.Id,
                    routePath: "/SystemManage/Role",
                    componentPath: null,
                    icon: "lucide:shield-check",
                    sortOrder: 2,
                    permissionCode: "");

                var menuMenu = new Menu(
                    menuName: "菜单管理",
                    menuType: MenuType.Menu,
                    parentId: systemMenu.Id,
                    routePath: "/SystemManage/Menu",
                    componentPath: null,
                    icon: "lucide:layout-list",
                    sortOrder: 2,
                    permissionCode: "");

                await context.Menus.AddRangeAsync([userMenu, roleMenu, menuMenu], cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}