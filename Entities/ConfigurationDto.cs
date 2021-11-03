using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace fxl.codes.kisekae.Entities
{
    [Table("configuration")]
    public class ConfigurationDto : IKisekaeFile
    {
        public string Data { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }
        [Column("border_index")] public int? BorderIndex { get; set; }
        public List<CelConfigDto> Cels { get; set; } = new();
        public List<PaletteDto> Palettes { get; set; } = new();
        public int Id { get; set; }
        [Column("kisekae_id")] public int KisekaeId { get; set; }
        public string Filename { get; set; }
    }
}