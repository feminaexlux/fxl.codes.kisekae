using fxl.codes.kisekae.data.Entities;
using Microsoft.EntityFrameworkCore;

namespace fxl.codes.kisekae.data
{
    public class KisekaeContext : DbContext
    {
        public KisekaeContext(DbContextOptions<KisekaeContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CelConfig>().Ignore(x => x.Sets);
            base.OnModelCreating(modelBuilder);
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