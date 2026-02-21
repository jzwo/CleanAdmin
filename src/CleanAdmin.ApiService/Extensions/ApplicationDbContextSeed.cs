using Microsoft.EntityFrameworkCore;
using CleanAdmin.Domain.AggregatesModel.MenuAggregate;
using CleanAdmin.Domain.AggregatesModel.UserAggregate;
using CleanAdmin.Infrastructure.Utils;

namespace CleanAdmin.ApiService.Extensions;

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
            if (!await context.Users.AnyAsync(u => u.Username == SystemDefaultSuperAdmin.Username, cancellationToken))
            {
                var superAdminUser = new User(
                    username: SystemDefaultSuperAdmin.Username,
                    passwordHash: passwordHasher.HashPassword(SystemDefaultSuperAdmin.Password),
                    realName: "super administrator",
                    email: "superadmin@example.com",
                    phone: "13900000000",
                    userRoles: [],
                    userPermissions: []);

                await context.Users.AddAsync(superAdminUser, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);

            if (!await context.Menus.AnyAsync(cancellationToken))
            {
                // 创建主菜单
                var homeMenu = new Menu(
                    menuName: "Home",
                    menuType: MenuType.Menu,
                    parentId: null,
                    routePath: "/",
                    componentPath: null,
                    icon: "lucide:home",
                    sortOrder: 1,
                    permissionCode: null);

                var systemMenu = new Menu(
                    menuName: "System Management",
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
                    menuName: "User Management",
                    menuType: MenuType.Menu,
                    parentId: systemMenu.Id,
                    routePath: "/SystemManage/User",
                    componentPath: null,
                    icon: "lucide:user-cog",
                    sortOrder: 1,
                    permissionCode: AppPermissions.System_Users_List);

                var roleMenu = new Menu(
                    menuName: "Role Management",
                    menuType: MenuType.Menu,
                    parentId: systemMenu.Id,
                    routePath: "/SystemManage/Role",
                    componentPath: null,
                    icon: "lucide:shield-check",
                    sortOrder: 2,
                    permissionCode: AppPermissions.System_Roles_List);

                var menuMenu = new Menu(
                    menuName: "Menu Management",
                    menuType: MenuType.Menu,
                    parentId: systemMenu.Id,
                    routePath: "/SystemManage/Menu",
                    componentPath: null,
                    icon: "lucide:layout-list",
                    sortOrder: 3,
                    permissionCode: AppPermissions.System_Menus_List);

                var userCreateButton = new Menu(
                    menuName: "Create User",
                    menuType: MenuType.Button,
                    parentId: userMenu.Id,
                    routePath: string.Empty,
                    componentPath: null,
                    icon: null,
                    sortOrder: 1,
                    permissionCode: AppPermissions.System_Users_Create);

                var userUpdateButton = new Menu(
                    menuName: "Edit User",
                    menuType: MenuType.Button,
                    parentId: userMenu.Id,
                    routePath: string.Empty,
                    componentPath: null,
                    icon: null,
                    sortOrder: 2,
                    permissionCode: AppPermissions.System_Users_Update);

                var userDeleteButton = new Menu(
                    menuName: "Delete User",
                    menuType: MenuType.Button,
                    parentId: userMenu.Id,
                    routePath: string.Empty,
                    componentPath: null,
                    icon: null,
                    sortOrder: 3,
                    permissionCode: AppPermissions.System_Users_Delete);

                var roleCreateButton = new Menu(
                    menuName: "Create Role",
                    menuType: MenuType.Button,
                    parentId: roleMenu.Id,
                    routePath: string.Empty,
                    componentPath: null,
                    icon: null,
                    sortOrder: 1,
                    permissionCode: AppPermissions.System_Roles_Create);

                var roleUpdateButton = new Menu(
                    menuName: "Edit Role",
                    menuType: MenuType.Button,
                    parentId: roleMenu.Id,
                    routePath: string.Empty,
                    componentPath: null,
                    icon: null,
                    sortOrder: 2,
                    permissionCode: AppPermissions.System_Roles_Update);

                var roleDeleteButton = new Menu(
                    menuName: "Delete Role",
                    menuType: MenuType.Button,
                    parentId: roleMenu.Id,
                    routePath: string.Empty,
                    componentPath: null,
                    icon: null,
                    sortOrder: 3,
                    permissionCode: AppPermissions.System_Roles_Delete);

                var roleAssignPermissionsButton = new Menu(
                    menuName: "Manage Role Permissions",
                    menuType: MenuType.Button,
                    parentId: roleMenu.Id,
                    routePath: string.Empty,
                    componentPath: null,
                    icon: null,
                    sortOrder: 4,
                    permissionCode: AppPermissions.System_Roles_ManagePermissions);

                var menuCreateButton = new Menu(
                    menuName: "Create Menu",
                    menuType: MenuType.Button,
                    parentId: menuMenu.Id,
                    routePath: string.Empty,
                    componentPath: null,
                    icon: null,
                    sortOrder: 1,
                    permissionCode: AppPermissions.System_Menus_Create);

                var menuUpdateButton = new Menu(
                    menuName: "Edit Menu",
                    menuType: MenuType.Button,
                    parentId: menuMenu.Id,
                    routePath: string.Empty,
                    componentPath: null,
                    icon: null,
                    sortOrder: 2,
                    permissionCode: AppPermissions.System_Menus_Update);

                var menuDeleteButton = new Menu(
                    menuName: "Delete Menu",
                    menuType: MenuType.Button,
                    parentId: menuMenu.Id,
                    routePath: string.Empty,
                    componentPath: null,
                    icon: null,
                    sortOrder: 3,
                    permissionCode: AppPermissions.System_Menus_Delete);

                var menuSetVisibilityButton = new Menu(
                    menuName: "Set Menu Visibility",
                    menuType: MenuType.Button,
                    parentId: menuMenu.Id,
                    routePath: string.Empty,
                    componentPath: null,
                    icon: null,
                    sortOrder: 4,
                    permissionCode: AppPermissions.System_Menus_SetVisibility);

                var menuSortOrderButton = new Menu(
                    menuName: "Adjust Menu Sort Order",
                    menuType: MenuType.Button,
                    parentId: menuMenu.Id,
                    routePath: string.Empty,
                    componentPath: null,
                    icon: null,
                    sortOrder: 5,
                    permissionCode: AppPermissions.System_Menus_UpdateSortOrder);

                await context.Menus.AddRangeAsync(
                [
                    userMenu,
                    roleMenu,
                    menuMenu,
                    userCreateButton,
                    userUpdateButton,
                    userDeleteButton,
                    roleCreateButton,
                    roleUpdateButton,
                    roleDeleteButton,
                    roleAssignPermissionsButton,
                    menuCreateButton,
                    menuUpdateButton,
                    menuDeleteButton,
                    menuSetVisibilityButton,
                    menuSortOrderButton
                ], cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}
