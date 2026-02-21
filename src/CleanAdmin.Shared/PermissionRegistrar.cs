using FluentPermissions.Core.Abstractions;
using FluentPermissions.Core.Builder;

namespace CleanAdmin.Shared;

// ReSharper disable once UnusedType.Global
[PermissionGenerationOptions(includeGroupAsPermission: false)]
public class PermissionRegistrar : IPermissionRegistrar
{
    private const string List = "List";
    private const string Create = "Create";
    private const string Update = "Update";
    private const string Delete = "Delete";

    public void Register(PermissionBuilder builder)
    {
        builder.DefineGroup("System", system =>
        {
            system.DefineGroup("Users", users =>
            {
                users.AddPermission(List);
                users.AddPermission(Create);
                users.AddPermission(Update);
                users.AddPermission(Delete);
            });
            system.DefineGroup("Roles", roles =>
            {
                roles.AddPermission(List);
                roles.AddPermission(Create);
                roles.AddPermission(Update);
                roles.AddPermission(Delete);
                roles.AddPermission("ManagePermissions");
            });
            system.DefineGroup("Menus", menus =>
            {
                menus.AddPermission(List);
                menus.AddPermission(Create);
                menus.AddPermission(Update);
                menus.AddPermission(Delete);
                menus.AddPermission("SetVisibility");
                menus.AddPermission("UpdateSortOrder");
            });
        });
    }
}
