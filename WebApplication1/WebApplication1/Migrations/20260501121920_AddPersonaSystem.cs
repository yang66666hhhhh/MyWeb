using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonaSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentPersonaTypeId",
                table: "Users",
                type: "char(36)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PersonaTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Icon = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    DefaultHomeRoute = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonaTypes", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PersonaMenuItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    PersonaTypeId = table.Column<Guid>(type: "char(36)", nullable: false),
                    MenuPath = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    MenuName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Icon = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    IsVisible = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ParentId = table.Column<Guid>(type: "char(36)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonaMenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonaMenuItems_PersonaMenuItems_ParentId",
                        column: x => x.ParentId,
                        principalTable: "PersonaMenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonaMenuItems_PersonaTypes_PersonaTypeId",
                        column: x => x.PersonaTypeId,
                        principalTable: "PersonaTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserPersonaRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    PersonaTypeId = table.Column<Guid>(type: "char(36)", nullable: false),
                    SwitchedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Remark = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPersonaRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPersonaRecords_PersonaTypes_PersonaTypeId",
                        column: x => x.PersonaTypeId,
                        principalTable: "PersonaTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPersonaRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PersonaMenuItems_MenuPath",
                table: "PersonaMenuItems",
                column: "MenuPath");

            migrationBuilder.CreateIndex(
                name: "IX_PersonaMenuItems_ParentId",
                table: "PersonaMenuItems",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonaMenuItems_PersonaTypeId",
                table: "PersonaMenuItems",
                column: "PersonaTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonaTypes_Code",
                table: "PersonaTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonaTypes_IsActive",
                table: "PersonaTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_UserPersonaRecords_PersonaTypeId",
                table: "UserPersonaRecords",
                column: "PersonaTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPersonaRecords_UserId",
                table: "UserPersonaRecords",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonaMenuItems");

            migrationBuilder.DropTable(
                name: "UserPersonaRecords");

            migrationBuilder.DropTable(
                name: "PersonaTypes");

            migrationBuilder.DropColumn(
                name: "CurrentPersonaTypeId",
                table: "Users");
        }
    }
}
