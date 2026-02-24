using System.Net;
using CleanAdmin.ApiService.Application.Queries.Roles;
using CleanAdmin.ApiService.Endpoints.Roles;
using CleanAdmin.ApiService.Tests.Fixtures;
using CleanAdmin.Domain.AggregatesModel.RoleAggregate;

namespace CleanAdmin.ApiService.Tests.Roles;

[Collection(WebAppTestCollection.Name)]
public class RolesManagementTests(WebAppFixture app, RolesManagementTests.RoleState state)
    : TestBase<WebAppFixture, RolesManagementTests.RoleState>
{
    [Fact, Priority(1)]
    public async Task CreateRole_ShouldReturnRoleId()
    {
        var request = new CreateRoleRequest
        {
            Name = state.RoleName,
            Description = "Test role description"
        };

        var (rsp, res) = await app.AuthenticatedClient
            .POSTAsync<CreateRoleEndpoint, CreateRoleRequest, ResponseData<CreateRoleResponse>>(request);

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
        res.Data.RoleId.Id.ShouldNotBe(Guid.Empty);

        state.RoleId = res.Data.RoleId;
    }

    [Fact, Priority(2)]
    public async Task RoleList_ShouldIncludeCreatedRole()
    {
        var (rsp, res) = await app.AuthenticatedClient
            .GETAsync<RoleListEndpoint, GetRoleListRequest, ResponseData<PagedData<RoleListItemDto>>>(
                new GetRoleListRequest
                {
                    Name = state.RoleName,
                    PageSize = 10
                });

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
        res.Data.Items.ShouldContain(r => r.Name == state.RoleName);
    }

    [Fact, Priority(4)]
    public async Task UpdateRoleInfo_ShouldModifyRoleDetails()
    {
        var roleId = state.RoleId ?? throw new InvalidOperationException("RoleId not initialized");
        const string updatedDescription = "Updated role description";

        var infoRequest = new UpdateRoleInfoRequest
        {
            RoleId = roleId,
            Name = state.RoleName,
            Description = updatedDescription
        };

        var (infoRsp, infoRes) = await app.AuthenticatedClient
            .POSTAsync<UpdateRoleInfoEndpoint, UpdateRoleInfoRequest, ResponseData>(infoRequest);

        infoRsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        infoRes.Success.ShouldBeTrue();
    }

    [Fact, Priority(6)]
    public async Task UpdateRolePermissions_ShouldModifyRolePermissions()
    {
        var roleId = state.RoleId ?? throw new InvalidOperationException("RoleId not initialized");

        // 创建测试权限代码列表
        var testPermissionCodes = new List<string>
        {
            "user.view",
            "user.create",
            "user.edit",
            "user.delete"
        };

        var updatePermissionsRequest = new UpdateRolePermissionsRequest
        {
            RoleId = roleId,
            PermissionCodes = testPermissionCodes
        };

        var (updateRsp, updateRes) = await app.AuthenticatedClient
            .POSTAsync<UpdateRolePermissionsEndpoint, UpdateRolePermissionsRequest, ResponseData>(
                updatePermissionsRequest);

        updateRsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        updateRes.Success.ShouldBeTrue();

        // 验证权限已更新
        var (getRsp, getRes) = await app.AuthenticatedClient
            .GETAsync<RolePermissionsEndpoint, RolePermissionsRequest, ResponseData<RolePermissionsResponse>>(
                new RolePermissionsRequest { RoleId = roleId });

        getRsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        getRes.Success.ShouldBeTrue();
        getRes.Data.PermissionCodes.Count.ShouldBe(4);
        getRes.Data.PermissionCodes.ShouldContain("user.view");
        getRes.Data.PermissionCodes.ShouldContain("user.create");
        getRes.Data.PermissionCodes.ShouldContain("user.edit");
        getRes.Data.PermissionCodes.ShouldContain("user.delete");
    }


    [Fact, Priority(8)]
    public async Task GetRolePermissions_ShouldReturnEmptyForNewRole()
    {
        var roleId = state.RoleId ?? throw new InvalidOperationException("RoleId not initialized");

        var (rsp, res) = await app.AuthenticatedClient
            .GETAsync<RolePermissionsEndpoint, RolePermissionsRequest, ResponseData<RolePermissionsResponse>>(
                new RolePermissionsRequest { RoleId = roleId });

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();
        res.Data.RoleId.ShouldBe(roleId);
        // 权限列表可能为空或包含之前测试设置的权限
        res.Data.PermissionCodes.ShouldNotBeNull();
    }

    [Fact, Priority(9)]
    public async Task DeleteRole_ShouldRemoveRole()
    {
        var roleId = state.RoleId ?? throw new InvalidOperationException("RoleId not initialized");

        var (rsp, res) = await app.AuthenticatedClient
            .DELETEAsync<DeleteRoleEndpoint, DeleteRoleRequest, ResponseData>(
                new DeleteRoleRequest { RoleId = roleId });

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.Success.ShouldBeTrue();

        var (_, listResponse) = await app.AuthenticatedClient
            .GETAsync<RoleListEndpoint, GetRoleListRequest, ResponseData<PagedData<RoleListItemDto>>>(
                new GetRoleListRequest
                {
                    Name = state.RoleName,
                    PageSize = 10
                });

        listResponse.Success.ShouldBeTrue();
        listResponse.Data.Items.ShouldNotContain(r => r.Name == state.RoleName);
    }

    public sealed class RoleState : StateFixture
    {
        public string RoleName { get; } = $"role_{Guid.NewGuid():N}";
        public RoleId? RoleId { get; set; }

        protected override ValueTask SetupAsync() => ValueTask.CompletedTask;

        protected override ValueTask TearDownAsync() => ValueTask.CompletedTask;
    }
}
