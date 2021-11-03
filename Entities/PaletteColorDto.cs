using System.ComponentModel.DataAnnotations.Schema;

namespace fxl.codes.kisekae.Entities
{
    [Table("palette_color")]
    public class PaletteColorDto
    {
        public int Id { get; set; }
        [Column("palette_id")] public int PaletteId { get; set; }
        public int Group { get; set; }
        public string Hex { get; set; }
    }
}