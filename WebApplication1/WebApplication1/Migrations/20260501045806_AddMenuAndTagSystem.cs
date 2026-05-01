using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuAndTagSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DailyPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    PlanDate = table.Column<DateTime>(type: "date", nullable: false),
                    Title = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Remark = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyPlans", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ExamMaterials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "varchar(10000)", maxLength: 10000, nullable: true),
                    Subject = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Tags = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamMaterials", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ExamMistakes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Question = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    Answer = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    Explanation = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    Subject = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Tags = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    LastReviewDate = table.Column<DateTime>(type: "date", nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamMistakes", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GrowthProjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "date", nullable: true),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
                    Progress = table.Column<int>(type: "int", nullable: false),
                    TaskCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Type = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowthProjects", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Habits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    HabitType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    TargetFrequency = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CurrentStreak = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LongestStreak = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalCheckIns = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastCheckInDate = table.Column<DateTime>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habits", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "IndustryTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Industry = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    IsDefault = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustryTemplates", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "KnowledgeArticles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "varchar(10000)", maxLength: 10000, nullable: true),
                    Category = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Tags = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    IsPublished = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeArticles", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Path = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Icon = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ParentId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RequiredPermissions = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItems_MenuItems_ParentId",
                        column: x => x.ParentId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PostgraduateTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    DueDate = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    Type = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostgraduateTasks", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Permissions = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    IsSystem = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MaxUsers = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WorkCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkCategories_WorkCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "WorkCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WorkImportBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    FileName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ImportType = table.Column<int>(type: "int", nullable: false),
                    TotalRows = table.Column<int>(type: "int", nullable: false),
                    SuccessRows = table.Column<int>(type: "int", nullable: false),
                    FailedRows = table.Column<int>(type: "int", nullable: false),
                    SkippedRows = table.Column<int>(type: "int", nullable: false),
                    DuplicateRows = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ImportStrategy = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    StartedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FinishedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkImportBatches", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WorkProjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    ProjectName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    ProjectCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    ProjectType = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CustomerName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "date", nullable: true),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkProjects", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WorkTaskTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    TypeName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    TypeCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTaskTypes", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HabitCheckIns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    HabitId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CheckInDate = table.Column<DateTime>(type: "date", nullable: false),
                    Remark = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitCheckIns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HabitCheckIns_Habits_HabitId",
                        column: x => x.HabitId,
                        principalTable: "Habits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TemplateFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    TemplateId = table.Column<Guid>(type: "char(36)", nullable: false),
                    FieldName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    FieldLabel = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    FieldType = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Options = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    IsRequired = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    DefaultValue = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateFields_IndustryTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "IndustryTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MenuTags",
                columns: table => new
                {
                    MenuItemId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TagId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuTags", x => new { x.MenuItemId, x.TagId });
                    table.ForeignKey(
                        name: "FK_MenuTags_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Username = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    RealName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Avatar = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastLoginAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastLoginIp = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Roles = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, defaultValue: "user"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WorkImportRows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    BatchId = table.Column<Guid>(type: "char(36)", nullable: false),
                    RowNumber = table.Column<int>(type: "int", nullable: false),
                    RawDate = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    RawWeekDay = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    RawProject = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    RawDevice = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    RawTaskType = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    RawContent = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    RawHours = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    RawRemark = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    ParsedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ParsedHours = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ValidationStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ErrorMessage = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    DuplicateStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ImportedWorkLogId = table.Column<Guid>(type: "char(36)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkImportRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkImportRows_WorkImportBatches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "WorkImportBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WorkDailyPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    PlanDate = table.Column<DateTime>(type: "date", nullable: false),
                    Title = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    Content = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    ProjectId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    StartTime = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    EndTime = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    EstimatedHours = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ActualHours = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ConvertedWorkLogId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Remark = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkDailyPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkDailyPlans_WorkProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "WorkProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WorkDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    ProjectId = table.Column<Guid>(type: "char(36)", nullable: true),
                    DeviceName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    DeviceCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    DeviceType = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkDevices_WorkProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "WorkProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WorkLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: true),
                    WorkDate = table.Column<DateTime>(type: "date", nullable: false),
                    WeekDay = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    ProjectId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    OriginalContent = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true),
                    Summary = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    TotalHours = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    SourceType = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ImportBatchId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Remark = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    TemplateId = table.Column<Guid>(type: "char(36)", nullable: true),
                    CategoryId = table.Column<Guid>(type: "char(36)", nullable: true),
                    IndustryTemplateId = table.Column<Guid>(type: "char(36)", nullable: true),
                    WorkCategoryId = table.Column<Guid>(type: "char(36)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkLogs_IndustryTemplates_IndustryTemplateId",
                        column: x => x.IndustryTemplateId,
                        principalTable: "IndustryTemplates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkLogs_IndustryTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "IndustryTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WorkLogs_WorkCategories_WorkCategoryId",
                        column: x => x.WorkCategoryId,
                        principalTable: "WorkCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkLogs_WorkProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "WorkProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserTags",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TagId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTags", x => new { x.UserId, x.TagId });
                    table.ForeignKey(
                        name: "FK_UserTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTags_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WorkLogDynamicValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    WorkLogId = table.Column<Guid>(type: "char(36)", nullable: false),
                    FieldId = table.Column<Guid>(type: "char(36)", nullable: false),
                    FieldName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    StringValue = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    NumberValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DateValue = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogDynamicValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkLogDynamicValues_WorkLogs_WorkLogId",
                        column: x => x.WorkLogId,
                        principalTable: "WorkLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WorkLogItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    WorkLogId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Content = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    TaskTypeId = table.Column<Guid>(type: "char(36)", nullable: true),
                    DeviceId = table.Column<Guid>(type: "char(36)", nullable: true),
                    ProgressPercent = table.Column<int>(type: "int", nullable: true),
                    Hours = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkLogItems_WorkDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "WorkDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WorkLogItems_WorkLogs_WorkLogId",
                        column: x => x.WorkLogId,
                        principalTable: "WorkLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkLogItems_WorkTaskTypes_TaskTypeId",
                        column: x => x.TaskTypeId,
                        principalTable: "WorkTaskTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlans_PlanDate",
                table: "DailyPlans",
                column: "PlanDate");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlans_Status",
                table: "DailyPlans",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ExamMaterials_Subject",
                table: "ExamMaterials",
                column: "Subject");

            migrationBuilder.CreateIndex(
                name: "IX_ExamMaterials_Type",
                table: "ExamMaterials",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ExamMaterials_UserId",
                table: "ExamMaterials",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamMistakes_Status",
                table: "ExamMistakes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ExamMistakes_Subject",
                table: "ExamMistakes",
                column: "Subject");

            migrationBuilder.CreateIndex(
                name: "IX_ExamMistakes_UserId",
                table: "ExamMistakes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GrowthProjects_Name",
                table: "GrowthProjects",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_GrowthProjects_Status",
                table: "GrowthProjects",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_GrowthProjects_UserId",
                table: "GrowthProjects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitCheckIns_CheckInDate",
                table: "HabitCheckIns",
                column: "CheckInDate");

            migrationBuilder.CreateIndex(
                name: "IX_HabitCheckIns_HabitId_CheckInDate",
                table: "HabitCheckIns",
                columns: new[] { "HabitId", "CheckInDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Habits_Name",
                table: "Habits",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Habits_Status",
                table: "Habits",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Habits_UserId",
                table: "Habits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IndustryTemplates_Name",
                table: "IndustryTemplates",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticles_Category",
                table: "KnowledgeArticles",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticles_Title",
                table: "KnowledgeArticles",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticles_UserId",
                table: "KnowledgeArticles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_ParentId",
                table: "MenuItems",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_Path",
                table: "MenuItems",
                column: "Path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuTags_TagId",
                table: "MenuTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_PostgraduateTasks_DueDate",
                table: "PostgraduateTasks",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_PostgraduateTasks_Status",
                table: "PostgraduateTasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PostgraduateTasks_UserId",
                table: "PostgraduateTasks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Code",
                table: "Roles",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplateFields_TemplateId_FieldName",
                table: "TemplateFields",
                columns: new[] { "TemplateId", "FieldName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Code",
                table: "Tenants",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTags_TagId",
                table: "UserTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkCategories_Code",
                table: "WorkCategories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkCategories_ParentId",
                table: "WorkCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkDailyPlans_PlanDate",
                table: "WorkDailyPlans",
                column: "PlanDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkDailyPlans_ProjectId",
                table: "WorkDailyPlans",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkDailyPlans_Status",
                table: "WorkDailyPlans",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkDevices_DeviceName",
                table: "WorkDevices",
                column: "DeviceName");

            migrationBuilder.CreateIndex(
                name: "IX_WorkDevices_ProjectId",
                table: "WorkDevices",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkDevices_Status",
                table: "WorkDevices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkImportBatches_Status",
                table: "WorkImportBatches",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkImportRows_BatchId",
                table: "WorkImportRows",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogDynamicValues_WorkLogId_FieldName",
                table: "WorkLogDynamicValues",
                columns: new[] { "WorkLogId", "FieldName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogItems_DeviceId",
                table: "WorkLogItems",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogItems_TaskTypeId",
                table: "WorkLogItems",
                column: "TaskTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogItems_WorkLogId",
                table: "WorkLogItems",
                column: "WorkLogId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_IndustryTemplateId",
                table: "WorkLogs",
                column: "IndustryTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_ProjectId",
                table: "WorkLogs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_Status",
                table: "WorkLogs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_TemplateId",
                table: "WorkLogs",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_UserId",
                table: "WorkLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_WorkCategoryId",
                table: "WorkLogs",
                column: "WorkCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_WorkDate",
                table: "WorkLogs",
                column: "WorkDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkProjects_ProjectName",
                table: "WorkProjects",
                column: "ProjectName");

            migrationBuilder.CreateIndex(
                name: "IX_WorkProjects_Status",
                table: "WorkProjects",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTaskTypes_TypeName",
                table: "WorkTaskTypes",
                column: "TypeName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyPlans");

            migrationBuilder.DropTable(
                name: "ExamMaterials");

            migrationBuilder.DropTable(
                name: "ExamMistakes");

            migrationBuilder.DropTable(
                name: "GrowthProjects");

            migrationBuilder.DropTable(
                name: "HabitCheckIns");

            migrationBuilder.DropTable(
                name: "KnowledgeArticles");

            migrationBuilder.DropTable(
                name: "MenuTags");

            migrationBuilder.DropTable(
                name: "PostgraduateTasks");

            migrationBuilder.DropTable(
                name: "TemplateFields");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTags");

            migrationBuilder.DropTable(
                name: "WorkDailyPlans");

            migrationBuilder.DropTable(
                name: "WorkImportRows");

            migrationBuilder.DropTable(
                name: "WorkLogDynamicValues");

            migrationBuilder.DropTable(
                name: "WorkLogItems");

            migrationBuilder.DropTable(
                name: "Habits");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WorkImportBatches");

            migrationBuilder.DropTable(
                name: "WorkDevices");

            migrationBuilder.DropTable(
                name: "WorkLogs");

            migrationBuilder.DropTable(
                name: "WorkTaskTypes");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "IndustryTemplates");

            migrationBuilder.DropTable(
                name: "WorkCategories");

            migrationBuilder.DropTable(
                name: "WorkProjects");
        }
    }
}
