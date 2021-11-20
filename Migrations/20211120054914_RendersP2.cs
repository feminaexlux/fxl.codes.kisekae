using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fxl.codes.kisekae.Migrations
{
    public partial class RendersP2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Renders_Cels_CelId",
                table: "Renders");

            migrationBuilder.DropForeignKey(
                name: "FK_Renders_Palettes_PaletteId",
                table: "Renders");

            migrationBuilder.DropIndex(
                name: "IX_Renders_CelId",
                table: "Renders");

            migrationBuilder.DropIndex(
                name: "IX_Renders_PaletteId",
                table: "Renders");

            migrationBuilder.DropColumn(
                name: "CelId",
                table: "Renders");

            migrationBuilder.DropColumn(
                name: "PaletteId",
                table: "Renders");

            migrationBuilder.AddColumn<int>(
                name: "RenderId",
                table: "CelConfigs",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CelConfigs_RenderId",
                table: "CelConfigs",
                column: "RenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CelConfigs_Renders_RenderId",
                table: "CelConfigs",
                column: "RenderId",
                principalTable: "Renders",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CelConfigs_Renders_RenderId",
                table: "CelConfigs");

            migrationBuilder.DropIndex(
                name: "IX_CelConfigs_RenderId",
                table: "CelConfigs");

            migrationBuilder.DropColumn(
                name: "RenderId",
                table: "CelConfigs");

            migrationBuilder.AddColumn<int>(
                name: "CelId",
                table: "Renders",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaletteId",
                table: "Renders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Renders_CelId",
                table: "Renders",
                column: "CelId");

            migrationBuilder.CreateIndex(
                name: "IX_Renders_PaletteId",
                table: "Renders",
                column: "PaletteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Renders_Cels_CelId",
                table: "Renders",
                column: "CelId",
                principalTable: "Cels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Renders_Palettes_PaletteId",
                table: "Renders",
                column: "PaletteId",
                principalTable: "Palettes",
                principalColumn: "Id");
        }
    }
}
