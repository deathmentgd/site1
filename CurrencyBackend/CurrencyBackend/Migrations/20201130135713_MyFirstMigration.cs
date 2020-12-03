using Microsoft.EntityFrameworkCore.Migrations;

namespace CurrencyBackend.Migrations
{
    public partial class MyFirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrencyList",
                columns: table => new
                {
                    CharCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<double>(type: "float", nullable: false),
                    Previous = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyList", x => x.CharCode);
                });

            migrationBuilder.CreateTable(
                name: "TrackedCurrencyList",
                columns: table => new
                {
                    CharCode = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedCurrencyList", x => x.CharCode);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyList");

            migrationBuilder.DropTable(
                name: "TrackedCurrencyList");
        }
    }
}
