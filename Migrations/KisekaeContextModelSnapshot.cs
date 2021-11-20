﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using fxl.codes.kisekae;

#nullable disable

namespace fxl.codes.kisekae.Migrations
{
    [DbContext(typeof(KisekaeContext))]
    partial class KisekaeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("fxl.codes.kisekae.Entities.Action", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ConfigurationId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ConfigurationId");

                    b.ToTable("Action");
                });

            modelBuilder.Entity("fxl.codes.kisekae.Entities.Cel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<byte[]>("Data")
                        .HasColumnType("bytea");

                    b.Property<string>("FileName")
                        .HasColumnType("text");

                    b.Property<int>("Height")
                        .HasColumnType("integer");

                    b.Property<int?>("KisekaeId")
                        .HasColumnType("integer");

                    b.Property<int>("OffsetX")
                        .HasColumnType("integer");

                    b.Property<int>("OffsetY")
                        .HasColumnType("integer");

                    b.Property<int>("Width")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("KisekaeId");

                    b.ToTable("Cels");
                });

            modelBuilder.Entity("fxl.codes.kisekae.Entities.CelConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("CelId")
                        .HasColumnType("integer");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<int?>("ConfigurationId")
                        .HasColumnType("integer");

                    b.Property<int>("Fix")
                        .HasColumnType("integer");

                    b.Property<int>("Mark")
                        .HasColumnType("integer");

                    b.Property<int>("PaletteGroup")
                        .HasColumnType("integer");

                    b.Property<int?>("PaletteId")
                        .HasColumnType("integer");

                    b.Property<int?>("RenderId")
                        .HasColumnType("integer");

                    b.Property<int>("Sets")
                        .HasColumnType("integer");

                    b.Property<int>("Transparency")
                        .HasColumnType("integer");

                    b.Property<int>("X")
                        .HasColumnType("integer");

                    b.Property<int>("Y")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CelId");

                    b.HasIndex("ConfigurationId");

                    b.HasIndex("PaletteId");

                    b.HasIndex("RenderId");

                    b.ToTable("CelConfigs");
                });

            modelBuilder.Entity("fxl.codes.kisekae.Entities.Configuration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BorderIndex")
                        .HasColumnType("integer");

                    b.Property<string>("Data")
                        .HasColumnType("text");

                    b.Property<int>("Height")
                        .HasColumnType("integer");

                    b.Property<int?>("KisekaeId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("Width")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("KisekaeId");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("fxl.codes.kisekae.Entities.Kisekae", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CheckSum")
                        .HasColumnType("text");

                    b.Property<string>("FileName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("KisekaeSets");
                });

            modelBuilder.Entity("fxl.codes.kisekae.Entities.Palette", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<byte[]>("Data")
                        .HasColumnType("bytea");

                    b.Property<string>("FileName")
                        .HasColumnType("text");

                    b.Property<int?>("KisekaeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("KisekaeId");

                    b.ToTable("Palettes");
                });

            modelBuilder.Entity("fxl.codes.kisekae.Entities.PaletteColor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Group")
                        .HasColumnType("integer");

                    b.Property<string>("Hex")
                        .HasColumnType("text");

                    b.Property<int?>("PaletteId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PaletteId");

                    b.ToTable("PaletteColors");
                });

            modelBuilder.Entity("fxl.codes.kisekae.Entities.Render", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<byte[]>("Image")
                        .HasColumnType("bytea");

                    b.HasKey("Id");

                    b.ToTable("Renders");
                });

            modelBuilder.Entity("fxl.codes.kisekae.Entities.Action", b =>
                {
                    b.HasOne("fxl.codes.kisekae.Entities.Configuration", null)
                        .WithMany("Actions")
                        .HasForeignKey("ConfigurationId");
                });

            modelBuilder.Entity("fxl.codes.kisekae.Entities.Cel", b =>
                {
                    b.HasOne("fxl.codes.kisekae.Entities.Kisekae", "Kisekae")
                        .WithMany("Cels")
                        .HasForeignKey("KisekaeId");

                    b.Navigation("Kisekae");
                });

            modelBuilder.Entity("fxl.codes.kisekae.Entities.CelConfig", b =>
                {
                    b.HasOne("fxl.codes.kisekae.Entities.Cel", "Cel")
                        .WithMany()
                        .HasForeignKey("CelId");

                    b.HasOne("fxl.codes.kisekae.Entities.Configuration", "Configuration")
                        .WithMany("Cels")
                        .HasForeignKey("ConfigurationId");

                    b.HasOne("fxl.codes.kisekae.Entities.Palette", "Palette")
                        .WithMany()
                        .HasForeignKey("PaletteId");

                    b.HasOne("fxl.codes.kisekae.Entities.Render", "Render")
                        .WithMany()
                        .HasForeignKey("RenderId");

                    b.Navigation("Cel");

                    b.Navigation("Configuration");

                    b.Navigation("Palette");

                    b.Navigation("Render");
                });

            modelBuilder.Entity("fxl.codes.kisekae.Entities.Configuration", b =>
                {
                    b.HasOne("fxl.codes.kisekae.Entities.Kisekae", "Kisekae")
                        .WithMany("Configurations")
                        .HasForeignKey("KisekaeId");

                    b.Navigation("Kisekae");
                });

            modelBuilder.Entity("fxl.codes.kisekae.Entities.Palette", b =>
                {
                    b.HasOne("fxl.codes.kisekae.Entities.Kisekae", "Kisekae")
                        .WithMany("Palettes")
                        .HasForeignKey("KisekaeId");

                    b.Navigation("Kisekae");
                });

            modelBuilder.Entity("fxl.codes.kisekae.Entities.PaletteColor", b =>
                {
                    b.HasOne("fxl.codes.kisekae.Entities.Palette", "Palette")
                        .WithMany("Colors")
                        .HasForeignKey("PaletteId");

                    b.Navigation("Palette");
                });

            modelBuilder.Entity("fxl.codes.kisekae.Entities.Configuration", b =>
                {
                    b.Navigation("Actions");

                    b.Navigation("Cels");
                });

            modelBuilder.Entity("fxl.codes.kisekae.Entities.Kisekae", b =>
                {
                    b.Navigation("Cels");

                    b.Navigation("Configurations");

                    b.Navigation("Palettes");
                });

            modelBuilder.Entity("fxl.codes.kisekae.Entities.Palette", b =>
                {
                    b.Navigation("Colors");
                });
#pragma warning restore 612, 618
        }
    }
}
