using Dapper.Contrib.Extensions;

namespace fxl.codes.kisekae.Entities
{
    [Table("cel")]
    public class CelDto : IKisekaeFile, IKisekaeParseable
    {
        public int Id { get; set; }
        public int KisekaeId { get; set; }
        public string Filename { get; set; }
        public byte[] Data { get; set; }
    }
}