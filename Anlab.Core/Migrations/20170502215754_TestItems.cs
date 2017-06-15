using Microsoft.EntityFrameworkCore.Migrations;

namespace Anlab.Core.Migrations
{
    public partial class TestItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TestItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Analysis = table.Column<string>(maxLength: 512, nullable: false),
                    ChargeSet = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(maxLength: 128, nullable: false),
                    ExternalCost = table.Column<decimal>(nullable: false),
                    FeeSchedule = table.Column<string>(maxLength: 7, nullable: false),
                    GroupType = table.Column<string>(nullable: true),
                    InternalCost = table.Column<decimal>(nullable: false),
                    Multiplier = table.Column<int>(nullable: false),
                    Multiplies = table.Column<bool>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    Public = table.Column<bool>(nullable: false),
                    TestType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestItems", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestItems");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 256);
        }
    }
}
