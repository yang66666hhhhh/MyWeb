using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddFeatureCodeToRoleMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FeatureCode",
                table: "RoleMenus",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_FeatureCode",
                table: "RoleMenus",
                column: "FeatureCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoleMenus_FeatureCode",
                table: "RoleMenus");

            migrationBuilder.DropColumn(
                name: "FeatureCode",
                table: "RoleMenus");
        }
    }
}
