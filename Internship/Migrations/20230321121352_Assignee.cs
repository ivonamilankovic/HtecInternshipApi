using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Internship.Migrations
{
    /// <inheritdoc />
    public partial class Assignee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssigneeId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AssigneeId",
                table: "Users",
                column: "AssigneeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_AssigneeId",
                table: "Users",
                column: "AssigneeId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_AssigneeId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_AssigneeId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "Users");
        }
    }
}
