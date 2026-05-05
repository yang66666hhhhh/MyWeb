using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleMenuTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoleMenus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    RoleCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    ParentId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Path = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Icon = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Component = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    IsVisible = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Permission = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Redirect = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    IsExternal = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Badge = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Tag = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMenus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleMenus_RoleMenus_ParentId",
                        column: x => x.ParentId,
                        principalTable: "RoleMenus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_ParentId",
                table: "RoleMenus",
                column: "ParentId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleMenus");
        }
    }
}
