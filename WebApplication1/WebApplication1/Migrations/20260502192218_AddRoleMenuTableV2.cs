using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleMenuTableV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoleMenus_RoleCode",
                table: "RoleMenus");

            migrationBuilder.DropIndex(
                name: "IX_RoleMenus_RoleCode_ParentId",
                table: "RoleMenus");

            migrationBuilder.DropIndex(
                name: "IX_RoleMenus_RoleCode_Path",
                table: "RoleMenus");

            migrationBuilder.DropColumn(
                name: "RoleCode",
                table: "RoleMenus");

            migrationBuilder.AddColumn<string>(
                name: "BindingType",
                table: "RoleMenus",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BindingValue",
                table: "RoleMenus",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_BindingType_BindingValue",
                table: "RoleMenus",
                columns: new[] { "BindingType", "BindingValue" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoleMenus_BindingType_BindingValue",
                table: "RoleMenus");

            migrationBuilder.DropColumn(
                name: "BindingType",
                table: "RoleMenus");

            migrationBuilder.DropColumn(
                name: "BindingValue",
                table: "RoleMenus");

            migrationBuilder.AddColumn<string>(
                name: "RoleCode",
                table: "RoleMenus",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_RoleCode",
                table: "RoleMenus",
                column: "RoleCode");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_RoleCode_ParentId",
                table: "RoleMenus",
                columns: new[] { "RoleCode", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_RoleCode_Path",
                table: "RoleMenus",
                columns: new[] { "RoleCode", "Path" },
                unique: true);
        }
    }
}
