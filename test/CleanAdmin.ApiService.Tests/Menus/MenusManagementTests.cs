using System.Net;
using CleanAdmin.ApiService.Application.Queries.Menus;
using CleanAdmin.ApiService.Endpoints.Menus;
using CleanAdmin.ApiService.Tests.Fixtures;
using CleanAdmin.Domain.AggregatesModel.MenuAggregate;

namespace CleanAdmin.ApiService.Tests.Menus;

[Collection(WebAppTestCollection.Name)]
public class MenusManagementTests(WebAppFixture app, MenusManagementTests.MenuState state)
    : TestBase<WebAppFixture, MenusManagementTests.MenuState>
{
    [Fact, Priority(1)]
    public async Task CreateMenu_ShouldReturnMenuId()
    {
        var request = new CreateMenuRequest
        {
            MenuName = state.MenuName,
            MenuType = MenuType.Menu,
            ParentId = null,
            RoutePath = "/test",
            ComponentPath = null,
            Icon = null,
            SortOrder = 100,
            PermissionCode = "test.view"
        };

        var (rsp, res) = await app.AuthenticatedClient
            .POSTAsync<CreateMenuEndpoint, CreateMenuRequest, ResponseData<CreateMenuResponse>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
        res.Data.MenuId.Id.ShouldNotBe(Guid.Empty);

        state.MenuId = res.Data.MenuId;
    }

    [Fact, Priority(4)]
    public async Task UpdateMenu_ShouldModifyMenuDetails()
    {
        var menuId = state.MenuId ?? throw new InvalidOperationException("MenuId not initialized");
        const string updatedRoutePath = "/test-updated";

        var updateRequest = new UpdateMenuRequest
        {
            MenuId = menuId,
            MenuName = state.MenuName,
            RoutePath = updatedRoutePath,
            ComponentPath = null,
            Icon = null,
            PermissionCode = "test.view"
        };

        var (updateRsp, updateRes) = await app.AuthenticatedClient
            .PUTAsync<UpdateMenuEndpoint, UpdateMenuRequest, ResponseData>(updateRequest);

        updateRsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        updateRes.Success.ShouldBeTrue();
    }

    [Fact, Priority(5)]
    public async Task UpdateMenuSortOrder_ShouldModifySortOrder()
    {
        var menuId = state.MenuId ?? throw new InvalidOperationException("MenuId not initialized");
        const int newSortOrder = 200;

        var request = new UpdateMenuSortOrderRequest
        {
            MenuId = menuId,
            NewSortOrder = newSortOrder
        };

        var (rsp, res) = await app.AuthenticatedClient
            .PATCHAsync<UpdateMenuSortOrderEndpoint, UpdateMenuSortOrderRequest, ResponseData>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
    }

    [Fact, Priority(6)]
    public async Task HideMenu_ShouldMakeMenuInvisible()
    {
        var menuId = state.MenuId ?? throw new InvalidOperationException("MenuId not initialized");

        var request = new HideMenuRequest
        {
            MenuId = menuId
        };

        var (rsp, res) = await app.AuthenticatedClient
            .PATCHAsync<HideMenuEndpoint, HideMenuRequest, ResponseData>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
    }

    [Fact, Priority(7)]
    public async Task ShowMenu_ShouldMakeMenuVisible()
    {
        var menuId = state.MenuId ?? throw new InvalidOperationException("MenuId not initialized");

        var request = new ShowMenuRequest
        {
            MenuId = menuId
        };

        var (rsp, res) = await app.AuthenticatedClient
            .PATCHAsync<ShowMenuEndpoint, ShowMenuRequest, ResponseData>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
    }

    [Fact, Priority(8)]
    public async Task DeleteMenu_ShouldMarkMenuAsDeleted()
    {
        var menuId = state.MenuId ?? throw new InvalidOperationException("MenuId not initialized");

        var request = new DeleteMenuRequest
        {
            MenuId = menuId
        };

        var (rsp, res) = await app.AuthenticatedClient
            .DELETEAsync<DeleteMenuEndpoint, DeleteMenuRequest, ResponseData>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
    }

    [Fact, Priority(9)]
    public async Task GetMenuTree_ShouldReturnHierarchicalStructure()
    {
        var (rsp, res) = await app.AuthenticatedClient
            .GETAsync<GetMenuTreeEndpoint, ResponseData<List<MenuTreeNodeDto>>>();
        
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
    }

    public sealed class MenuState : StateFixture
    {
        public string MenuName { get; } = $"Test Menu {Guid.NewGuid():N}";
        public MenuId? MenuId { get; set; }

        protected override ValueTask SetupAsync() => ValueTask.CompletedTask;

        protected override ValueTask TearDownAsync() => ValueTask.CompletedTask;
    }
}
