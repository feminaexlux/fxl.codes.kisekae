using fxl.codes.kisekae.data.Entities;
using Microsoft.EntityFrameworkCore;

namespace fxl.codes.kisekae.data;

public class KisekaeContext(DbContextOptions<KisekaeContext> options) : DbContext(options)
{
    public DbSet<Kisekae> KisekaeSets { get; init; }
    public DbSet<Configuration> Configurations { get; init; }
    public DbSet<Cel> Cels { get; init; }
    public DbSet<CelConfig> CelConfigs { get; init; }
    public DbSet<Render> Renders { get; init; }
    public DbSet<Palette> Palettes { get; init; }
    public DbSet<PaletteColor> PaletteColors { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CelConfig>().Ignore(x => x.Sets);
        base.OnModelCreating(modelBuilder);
    }
}