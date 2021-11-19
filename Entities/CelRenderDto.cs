using System.ComponentModel.DataAnnotations.Schema;

namespace fxl.codes.kisekae.Entities
{
    [Table("cel_render")]
    public class CelRenderDto
    {
        public int Id { get; set; }
        [Column("cel_id")] public int CelId { get; set; }
        [Column("palette_id")] public int PaletteId { get; set; }
        public byte[] Data { get; set; }
    }
}