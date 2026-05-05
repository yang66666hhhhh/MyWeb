using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleMenuBaseMenuColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<bool>(
                name: "IsBaseMenu",
                table: "RoleMenus",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MinRoleLevel",
                table: "RoleMenus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PersonaTag",
                table: "RoleMenus",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_MinRoleLevel",
                table: "RoleMenus",
                column: "MinRoleLevel");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_PersonaTag",
                table: "RoleMenus",
                column: "PersonaTag");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoleMenus_MinRoleLevel",
                table: "RoleMenus");

            migrationBuilder.DropIndex(
                name: "IX_RoleMenus_PersonaTag",
                table: "RoleMenus");

            migrationBuilder.DropColumn(
                name: "IsBaseMenu",
                table: "RoleMenus");

            migrationBuilder.DropColumn(
                name: "MinRoleLevel",
                table: "RoleMenus");

            migrationBuilder.DropColumn(
                name: "PersonaTag",
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
    }
}
