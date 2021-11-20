﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace fxl.codes.kisekae.Migrations
{
    public partial class Startup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KisekaeSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    CheckSum = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KisekaeSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    Data = table.Column<byte[]>(type: "bytea", nullable: true),
                    OffsetX = table.Column<int>(type: "integer", nullable: false),
                    OffsetY = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    KisekaeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cels_KisekaeSets_KisekaeId",
                        column: x => x.KisekaeId,
                        principalTable: "KisekaeSets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    KisekaeId = table.Column<int>(type: "integer", nullable: true),
                    Data = table.Column<string>(type: "text", nullable: true),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    BorderIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Configurations_KisekaeSets_KisekaeId",
                        column: x => x.KisekaeId,
                        principalTable: "KisekaeSets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Palettes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KisekaeId = table.Column<int>(type: "integer", nullable: true),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    Data = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Palettes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Palettes_KisekaeSets_KisekaeId",
                        column: x => x.KisekaeId,
                        principalTable: "KisekaeSets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Action",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ConfigurationId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Action", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Action_Configurations_ConfigurationId",
                        column: x => x.ConfigurationId,
                        principalTable: "Configurations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CelConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CelId = table.Column<int>(type: "integer", nullable: true),
                    ConfigurationId = table.Column<int>(type: "integer", nullable: true),
                    Mark = table.Column<int>(type: "integer", nullable: false),
                    Fix = table.Column<int>(type: "integer", nullable: false),
                    PaletteId = table.Column<int>(type: "integer", nullable: true),
                    PaletteGroup = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    Transparency = table.Column<int>(type: "integer", nullable: false),
                    Sets = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CelConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CelConfigs_Cels_CelId",
                        column: x => x.CelId,
                        principalTable: "Cels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CelConfigs_Configurations_ConfigurationId",
                        column: x => x.ConfigurationId,
                        principalTable: "Configurations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CelConfigs_Palettes_PaletteId",
                        column: x => x.PaletteId,
                        principalTable: "Palettes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PaletteColors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaletteId = table.Column<int>(type: "integer", nullable: true),
                    Group = table.Column<int>(type: "integer", nullable: false),
                    Hex = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaletteColors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaletteColors_Palettes_PaletteId",
                        column: x => x.PaletteId,
                        principalTable: "Palettes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Renders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CelId = table.Column<int>(type: "integer", nullable: true),
                    PaletteId = table.Column<int>(type: "integer", nullable: true),
                    Image = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Renders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Renders_Cels_CelId",
                        column: x => x.CelId,
                        principalTable: "Cels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Renders_Palettes_PaletteId",
                        column: x => x.PaletteId,
                        principalTable: "Palettes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Action_ConfigurationId",
                table: "Action",
                column: "ConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_CelConfigs_CelId",
                table: "CelConfigs",
                column: "CelId");

            migrationBuilder.CreateIndex(
                name: "IX_CelConfigs_ConfigurationId",
                table: "CelConfigs",
                column: "ConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_CelConfigs_PaletteId",
                table: "CelConfigs",
                column: "PaletteId");

            migrationBuilder.CreateIndex(
                name: "IX_Cels_KisekaeId",
                table: "Cels",
                column: "KisekaeId");

            migrationBuilder.CreateIndex(
                name: "IX_Configurations_KisekaeId",
                table: "Configurations",
                column: "KisekaeId");

            migrationBuilder.CreateIndex(
                name: "IX_PaletteColors_PaletteId",
                table: "PaletteColors",
                column: "PaletteId");

            migrationBuilder.CreateIndex(
                name: "IX_Palettes_KisekaeId",
                table: "Palettes",
                column: "KisekaeId");

            migrationBuilder.CreateIndex(
                name: "IX_Renders_CelId",
                table: "Renders",
                column: "CelId");

            migrationBuilder.CreateIndex(
                name: "IX_Renders_PaletteId",
                table: "Renders",
                column: "PaletteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Action");

            migrationBuilder.DropTable(
                name: "CelConfigs");

            migrationBuilder.DropTable(
                name: "PaletteColors");

            migrationBuilder.DropTable(
                name: "Renders");

            migrationBuilder.DropTable(
                name: "Configurations");

            migrationBuilder.DropTable(
                name: "Cels");

            migrationBuilder.DropTable(
                name: "Palettes");

            migrationBuilder.DropTable(
                name: "KisekaeSets");
        }
    }
}
