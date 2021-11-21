using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fxl.codes.kisekae.Migrations
{
    public partial class UseBackgroundHex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BorderIndex",
                table: "Configurations");

            migrationBuilder.AddColumn<string>(
                name: "BackgroundColorHex",
                table: "Configurations",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundColorHex",
                table: "Configurations");

            migrationBuilder.AddColumn<int>(
                name: "BorderIndex",
                table: "Configurations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
