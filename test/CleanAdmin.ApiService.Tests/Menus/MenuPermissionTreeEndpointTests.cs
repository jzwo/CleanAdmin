using System.Net;
using CleanAdmin.ApiService.Application.Queries.Menus;
using CleanAdmin.ApiService.Endpoints.Menus;
using CleanAdmin.ApiService.Tests.Fixtures;
using CleanAdmin.Domain.AggregatesModel.MenuAggregate;

namespace CleanAdmin.ApiService.Tests.Menus;

[Collection(WebAppTestCollection.Name)]
public class MenuPermissionTreeEndpointTests(WebAppFixture app) : TestBase<WebAppFixture>
{
    [Fact]
    public async Task GetMenuPermissionTree_ShouldReturnAllMenus_AndKeepPermissionCodeAsIs()
    {
        var validPermissionCode = $"integration.valid.permission.{Guid.NewGuid():N}";

        var invalidPermissionCode = "integration.invalid.permission";
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var (createRootRsp, createRootRes) = await app.AuthenticatedClient
            .POSTAsync<CreateMenuEndpoint, CreateMenuRequest, ResponseData<CreateMenuResponse>>(
                new CreateMenuRequest
                {
                    MenuName = $"IT-Permissions-Root-{suffix}",
                    MenuType = MenuType.Directory,
                    ParentId = null,
                    RoutePath = $"/it/perm-root-{suffix}",
                    SortOrder = 9000,
                    PermissionCode = null
                });

        createRootRsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        createRootRes.Success.ShouldBeTrue();
        var rootMenuId = createRootRes.Data.MenuId;

        var (createValidRsp, createValidRes) = await app.AuthenticatedClient
            .POSTAsync<CreateMenuEndpoint, CreateMenuRequest, ResponseData<CreateMenuResponse>>(
                new CreateMenuRequest
                {
                    MenuName = $"IT-Permissions-Valid-{suffix}",
                    MenuType = MenuType.Menu,
                    ParentId = rootMenuId,
                    RoutePath = $"/it/perm-valid-{suffix}",
                    SortOrder = 9001,
                    PermissionCode = validPermissionCode
                });

        createValidRsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        createValidRes.Success.ShouldBeTrue();
        var validMenuId = createValidRes.Data.MenuId;

        var (createInvalidRsp, createInvalidRes) = await app.AuthenticatedClient
            .POSTAsync<CreateMenuEndpoint, CreateMenuRequest, ResponseData<CreateMenuResponse>>(
                new CreateMenuRequest
                {
                    MenuName = $"IT-Permissions-Invalid-{suffix}",
                    MenuType = MenuType.Menu,
                    ParentId = rootMenuId,
                    RoutePath = $"/it/perm-invalid-{suffix}",
                    SortOrder = 9002,
                    PermissionCode = invalidPermissionCode
                });

        createInvalidRsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        createInvalidRes.Success.ShouldBeTrue();

        var (createNoPermissionRsp, createNoPermissionRes) = await app.AuthenticatedClient
            .POSTAsync<CreateMenuEndpoint, CreateMenuRequest, ResponseData<CreateMenuResponse>>(
                new CreateMenuRequest
                {
                    MenuName = $"IT-Permissions-NoCode-{suffix}",
                    MenuType = MenuType.Menu,
                    ParentId = rootMenuId,
                    RoutePath = $"/it/perm-nocode-{suffix}",
                    SortOrder = 9003,
                    PermissionCode = null
                });

        createNoPermissionRsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        createNoPermissionRes.Success.ShouldBeTrue();
        var noPermissionMenuId = createNoPermissionRes.Data.MenuId;

        var (rsp, res) = await app.AuthenticatedClient
            .GETAsync<GetMenuPermissionTreeEndpoint, ResponseData<List<MenuPermissionTreeNodeDto>>>();

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
        res.Data.ShouldNotBeNull();

        var allNodes = Flatten(res.Data).ToList();
        allNodes.ShouldContain(n => n.MenuId == rootMenuId);
        allNodes.ShouldContain(n => n.MenuId == validMenuId && n.PermissionCode == validPermissionCode);
        allNodes.ShouldContain(n => n.MenuId == noPermissionMenuId && n.PermissionCode == null);
        allNodes.ShouldContain(n => n.PermissionCode == invalidPermissionCode);
    }

    private static IEnumerable<MenuPermissionTreeNodeDto> Flatten(IEnumerable<MenuPermissionTreeNodeDto> nodes)
    {
        foreach (var node in nodes)
        {
            yield return node;
            foreach (var child in Flatten(node.Children))
            {
                yield return child;
            }
        }
    }
}
