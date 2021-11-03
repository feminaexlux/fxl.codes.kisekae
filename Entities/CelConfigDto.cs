using System.ComponentModel.DataAnnotations.Schema;

namespace fxl.codes.kisekae.Entities
{
    [Table("cel_config")]
    public class CelConfigDto
    {
        public int Id { get; set; }
        [Column("cel_id")] public int CelId { get; set; }
        [Column("config_id")] public int ConfigId { get; set; }
        public int Group { get; set; }
        public int Fix { get; set; }
        public Set Sets { get; set; } = Set.Unset;
        public int Transparency { get; set; }
        public string Comment { get; set; }
        [Column("palette_id")] public int PaletteId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}