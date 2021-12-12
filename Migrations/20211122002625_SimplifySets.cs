using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fxl.codes.kisekae.Migrations
{
    public partial class SimplifySets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Palettes");

            migrationBuilder.DropColumn(
                name: "Sets",
                table: "CelConfigs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Palettes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sets",
                table: "CelConfigs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
