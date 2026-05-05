using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiPersonaAndPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPersonaTypeId",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Roles",
                table: "Users",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "member",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldDefaultValue: "user");

            migrationBuilder.AddColumn<string>(
                name: "MenuCategory",
                table: "RoleMenus",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "General");

            migrationBuilder.CreateTable(
                name: "MenuActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    MenuId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ActionCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    ActionName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuActions_RoleMenus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "RoleMenus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false),
                    MenuId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ActionCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    IsAllowed = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_RoleMenus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "RoleMenus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserPersonas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    PersonaTypeId = table.Column<Guid>(type: "char(36)", nullable: false),
                    IsPrimary = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPersonas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPersonas_PersonaTypes_PersonaTypeId",
                        column: x => x.PersonaTypeId,
                        principalTable: "PersonaTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPersonas_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_MenuCategory",
                table: "RoleMenus",
                column: "MenuCategory");

            migrationBuilder.CreateIndex(
                name: "IX_MenuActions_MenuId_ActionCode",
                table: "MenuActions",
                columns: new[] { "MenuId", "ActionCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_MenuId",
                table: "RolePermissions",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_MenuId_ActionCode",
                table: "RolePermissions",
                columns: new[] { "RoleId", "MenuId", "ActionCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPersonas_PersonaTypeId",
                table: "UserPersonas",
                column: "PersonaTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPersonas_UserId_PersonaTypeId",
                table: "UserPersonas",
                columns: new[] { "UserId", "PersonaTypeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuActions");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserPersonas");

            migrationBuilder.DropIndex(
                name: "IX_RoleMenus_MenuCategory",
                table: "RoleMenus");

            migrationBuilder.DropColumn(
                name: "MenuCategory",
                table: "RoleMenus");

            migrationBuilder.AlterColumn<string>(
                name: "Roles",
                table: "Users",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "user",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldDefaultValue: "member");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentPersonaTypeId",
                table: "Users",
                type: "char(36)",
                nullable: true);
        }
    }
}
