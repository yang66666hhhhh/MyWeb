using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToDailyPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "DailyPlans",
                type: "char(36)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlans_UserId",
                table: "DailyPlans",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DailyPlans_UserId",
                table: "DailyPlans");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DailyPlans");
        }
    }
}
