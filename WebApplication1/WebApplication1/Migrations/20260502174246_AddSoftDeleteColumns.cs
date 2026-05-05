using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkCategories_WorkCategories_ParentId",
                table: "WorkCategories");

            migrationBuilder.DropIndex(
                name: "IX_WorkCategories_Code",
                table: "WorkCategories");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "WorkTaskTypes",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WorkTaskTypes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "WorkProjects",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WorkProjects",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "WorkLogs",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WorkLogs",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "WorkLogItems",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WorkLogItems",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "WorkLogDynamicValues",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WorkLogDynamicValues",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "WorkImportRows",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WorkImportRows",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "WorkImportBatches",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WorkImportBatches",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "WorkDevices",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WorkDevices",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "WorkDailyPlans",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WorkDailyPlans",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WorkCategories",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "WorkCategories",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "WorkCategories",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "WorkCategories",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WorkCategories",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "UserTypes",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserTypes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "UserPersonaRecords",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserPersonaRecords",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Tenants",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Tenants",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "TemplateFields",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TemplateFields",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Tags",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Tags",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "PostgraduateTasks",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PostgraduateTasks",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "PersonaTypes",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PersonaTypes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "PersonaMenuItems",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PersonaMenuItems",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "MenuItems",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MenuItems",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "MenuConfigs",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MenuConfigs",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "KnowledgeArticles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "KnowledgeArticles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "IndustryTemplates",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "IndustryTemplates",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Habits",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Habits",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "HabitCheckIns",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "HabitCheckIns",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "GrowthProjects",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "GrowthProjects",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ExamMistakes",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ExamMistakes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ExamMaterials",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ExamMaterials",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "DailyPlans",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DailyPlans",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AiReports",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AiReports",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AiPlans",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AiPlans",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AiChatSessions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AiChatSessions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AiChatMessages",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AiChatMessages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkCategories_WorkCategories_ParentId",
                table: "WorkCategories",
                column: "ParentId",
                principalTable: "WorkCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkCategories_WorkCategories_ParentId",
                table: "WorkCategories");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "WorkTaskTypes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WorkTaskTypes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "WorkProjects");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WorkProjects");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "WorkLogs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WorkLogs");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "WorkLogItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WorkLogItems");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "WorkLogDynamicValues");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WorkLogDynamicValues");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "WorkImportRows");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WorkImportRows");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "WorkImportBatches");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WorkImportBatches");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "WorkDevices");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WorkDevices");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "WorkDailyPlans");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WorkDailyPlans");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "WorkCategories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WorkCategories");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "UserTypes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserTypes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "UserPersonaRecords");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserPersonaRecords");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TemplateFields");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TemplateFields");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "PostgraduateTasks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PostgraduateTasks");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "PersonaTypes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PersonaTypes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "PersonaMenuItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PersonaMenuItems");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "MenuConfigs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MenuConfigs");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "KnowledgeArticles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "KnowledgeArticles");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "IndustryTemplates");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "IndustryTemplates");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "HabitCheckIns");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "HabitCheckIns");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "GrowthProjects");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "GrowthProjects");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ExamMistakes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ExamMistakes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ExamMaterials");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ExamMaterials");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "DailyPlans");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DailyPlans");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AiReports");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AiReports");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AiPlans");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AiPlans");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AiChatSessions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AiChatSessions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AiChatMessages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AiChatMessages");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WorkCategories",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "WorkCategories",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "WorkCategories",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.CreateIndex(
                name: "IX_WorkCategories_Code",
                table: "WorkCategories",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkCategories_WorkCategories_ParentId",
                table: "WorkCategories",
                column: "ParentId",
                principalTable: "WorkCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
