using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace fxl.codes.kisekae.Migrations
{
    public partial class Positions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "X",
                table: "CelConfigs");

            migrationBuilder.DropColumn(
                name: "Y",
                table: "CelConfigs");

            migrationBuilder.CreateTable(
                name: "CelPosition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Set = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    CelConfigId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CelPosition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CelPosition_CelConfigs_CelConfigId",
                        column: x => x.CelConfigId,
                        principalTable: "CelConfigs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CelPosition_CelConfigId",
                table: "CelPosition",
                column: "CelConfigId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CelPosition");

            migrationBuilder.AddColumn<int>(
                name: "X",
                table: "CelConfigs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Y",
                table: "CelConfigs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
