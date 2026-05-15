using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectRelationToImplLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImplLogs_WorkLogTemplates_TemplateId",
                table: "ImplLogs");

            migrationBuilder.AlterColumn<string>(
                name: "WeekDay",
                table: "ImplLogs",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalHours",
                table: "ImplLogs",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "ImplLogs",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectName",
                table: "ImplLogs",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PersonaCode",
                table: "ImplLogs",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExtraData",
                table: "ImplLogs",
                type: "varchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ImplLogs",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "ImplLogs",
                type: "char(36)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImplLogs_ProjectId",
                table: "ImplLogs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ImplLogs_UserId",
                table: "ImplLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ImplLogs_WorkDate",
                table: "ImplLogs",
                column: "WorkDate");

            migrationBuilder.AddForeignKey(
                name: "FK_ImplLogs_WorkLogTemplates_TemplateId",
                table: "ImplLogs",
                column: "TemplateId",
                principalTable: "WorkLogTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ImplLogs_WorkProjects_ProjectId",
                table: "ImplLogs",
                column: "ProjectId",
                principalTable: "WorkProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImplLogs_WorkLogTemplates_TemplateId",
                table: "ImplLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ImplLogs_WorkProjects_ProjectId",
                table: "ImplLogs");

            migrationBuilder.DropIndex(
                name: "IX_ImplLogs_ProjectId",
                table: "ImplLogs");

            migrationBuilder.DropIndex(
                name: "IX_ImplLogs_UserId",
                table: "ImplLogs");

            migrationBuilder.DropIndex(
                name: "IX_ImplLogs_WorkDate",
                table: "ImplLogs");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ImplLogs");

            migrationBuilder.AlterColumn<string>(
                name: "WeekDay",
                table: "ImplLogs",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalHours",
                table: "ImplLogs",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "ImplLogs",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "ProjectName",
                table: "ImplLogs",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PersonaCode",
                table: "ImplLogs",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExtraData",
                table: "ImplLogs",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ImplLogs",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddForeignKey(
                name: "FK_ImplLogs_WorkLogTemplates_TemplateId",
                table: "ImplLogs",
                column: "TemplateId",
                principalTable: "WorkLogTemplates",
                principalColumn: "Id");
        }
    }
}
