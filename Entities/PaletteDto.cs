using System.ComponentModel.DataAnnotations.Schema;

namespace fxl.codes.kisekae.Entities
{
    [Table("palette")]
    public class PaletteDto : IKisekaeFile, IKisekaeParseable
    {
        public string Comment { get; set; }
        public int Id { get; set; }
        [Column("kisekae_id")] public int KisekaeId { get; set; }
        public string Filename { get; set; }
        public byte[] Data { get; set; }
    }
}