using fxl.codes.kisekae.Entities;
using Microsoft.EntityFrameworkCore;

namespace fxl.codes.kisekae
{
    public class KisekaeContext : DbContext
    {
        public KisekaeContext(DbContextOptions<KisekaeContext> options) : base(options)
        {
        }

        public DbSet<Kisekae> KisekaeSets { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<Cel> Cels { get; set; }
        public DbSet<CelConfig> CelConfigs { get; set; }
        public DbSet<Render> Renders { get; set; }
        public DbSet<Palette> Palettes { get; set; }
        public DbSet<PaletteColor> PaletteColors { get; set; }
    }
}