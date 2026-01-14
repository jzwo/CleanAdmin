using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NcpAdminBlazor.MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class Menus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "menus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "菜单ID"),
                    MenuName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "菜单名称"),
                    MenuType = table.Column<int>(type: "int", nullable: false, comment: "菜单类型"),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true, comment: "父菜单ID"),
                    RoutePath = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "路由路径"),
                    ComponentPath = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "组件路径"),
                    Icon = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, comment: "菜单图标"),
                    SortOrder = table.Column<int>(type: "integer", nullable: false, comment: "排序号"),
                    IsExternal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "是否为外链"),
                    IsVisible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "是否可见"),
                    PermissionCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, comment: "权限码"),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0, comment: "菜单状态"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "创建时间"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, comment: "是否删除"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "删除时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_menus_IsDeleted",
                table: "menus",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_menus_ParentId",
                table: "menus",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_menus_ParentId_SortOrder",
                table: "menus",
                columns: new[] { "ParentId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_menus_PermissionCode",
                table: "menus",
                column: "PermissionCode");

            migrationBuilder.CreateIndex(
                name: "IX_menus_RoutePath",
                table: "menus",
                column: "RoutePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "menus");
        }
    }
}
