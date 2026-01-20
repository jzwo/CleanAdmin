namespace NcpAdminAntBlazor.Client.Services;

public class MenuItemModel
{
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public List<MenuItemModel> Children { get; set; } = new();
    public bool IsSubMenu => Children.Count > 0;
}

public interface IMenuService
{
    List<MenuItemModel> GetMenuItems();
}

public class MenuService : IMenuService
{
    public List<MenuItemModel> GetMenuItems()
    {
        return new List<MenuItemModel>
        {
            new MenuItemModel
            {
                Key = "dashboard",
                Title = "仪表盘",
                Icon = "pie-chart",
                Route = "/"
            },
            new MenuItemModel
            {
                Key = "desktop",
                Title = "工作台",
                Icon = "desktop",
                Route = "/desktop"
            },
            new MenuItemModel
            {
                Key = "user",
                Title = "用户管理",
                Icon = "user",
                Children = new List<MenuItemModel>
                {
                    new MenuItemModel
                    {
                        Key = "user-list",
                        Title = "用户列表",
                        Route = "/users"
                    },
                    new MenuItemModel
                    {
                        Key = "user-roles",
                        Title = "角色管理",
                        Route = "/roles"
                    },
                    new MenuItemModel
                    {
                        Key = "user-permissions",
                        Title = "权限管理",
                        Route = "/permissions"
                    }
                }
            },
            new MenuItemModel
            {
                Key = "team",
                Title = "团队管理",
                Icon = "team",
                Children = new List<MenuItemModel>
                {
                    new MenuItemModel
                    {
                        Key = "team-list",
                        Title = "团队列表",
                        Route = "/teams"
                    },
                    new MenuItemModel
                    {
                        Key = "team-members",
                        Title = "成员管理",
                        Route = "/team-members"
                    }
                }
            },
            new MenuItemModel
            {
                Key = "files",
                Title = "文件管理",
                Icon = "file",
                Route = "/files"
            }
        };
    }
}