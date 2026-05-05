using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkLogTemplateAndExtraData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExtraData",
                table: "WorkLogs",
                type: "varchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonaCode",
                table: "WorkLogs",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WorkLogTemplateId",
                table: "WorkLogs",
                type: "char(36)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkLogTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    PersonaCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    FieldDefinitions = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogTemplates", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_PersonaCode",
                table: "WorkLogs",
                column: "PersonaCode");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_WorkLogTemplateId",
                table: "WorkLogs",
                column: "WorkLogTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogTemplates_PersonaCode",
                table: "WorkLogTemplates",
                column: "PersonaCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkLogs_WorkLogTemplates_WorkLogTemplateId",
                table: "WorkLogs",
                column: "WorkLogTemplateId",
                principalTable: "WorkLogTemplates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkLogs_WorkLogTemplates_WorkLogTemplateId",
                table: "WorkLogs");

            migrationBuilder.DropTable(
                name: "WorkLogTemplates");

            migrationBuilder.DropIndex(
                name: "IX_WorkLogs_PersonaCode",
                table: "WorkLogs");

            migrationBuilder.DropIndex(
                name: "IX_WorkLogs_WorkLogTemplateId",
                table: "WorkLogs");

            migrationBuilder.DropColumn(
                name: "ExtraData",
                table: "WorkLogs");

            migrationBuilder.DropColumn(
                name: "PersonaCode",
                table: "WorkLogs");

            migrationBuilder.DropColumn(
                name: "WorkLogTemplateId",
                table: "WorkLogs");
        }
    }
}
