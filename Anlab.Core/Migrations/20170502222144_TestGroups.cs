using Microsoft.EntityFrameworkCore.Migrations;

namespace Anlab.Core.Migrations
{
    public partial class TestGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestType",
                table: "TestItems");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "TestItems",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "TestItems",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "SetupCost",
                table: "TestItems",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "TestItems");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "TestItems");

            migrationBuilder.DropColumn(
                name: "SetupCost",
                table: "TestItems");

            migrationBuilder.AddColumn<string>(
                name: "TestType",
                table: "TestItems",
                nullable: true);
        }
    }
}
