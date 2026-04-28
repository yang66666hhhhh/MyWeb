using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class InitialWorkTables : Migration
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
                    PlanDate = table.Column<DateOnly>(type: "date", nullable: false),
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
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
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
                    WorkDate = table.Column<DateOnly>(type: "date", nullable: false),
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
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkLogs_WorkProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "WorkProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "IX_WorkLogs_ProjectId",
                table: "WorkLogs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_Status",
                table: "WorkLogs",
                column: "Status");

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
                name: "WorkImportRows");

            migrationBuilder.DropTable(
                name: "WorkLogItems");

            migrationBuilder.DropTable(
                name: "WorkImportBatches");

            migrationBuilder.DropTable(
                name: "WorkDevices");

            migrationBuilder.DropTable(
                name: "WorkLogs");

            migrationBuilder.DropTable(
                name: "WorkTaskTypes");

            migrationBuilder.DropTable(
                name: "WorkProjects");
        }
    }
}
