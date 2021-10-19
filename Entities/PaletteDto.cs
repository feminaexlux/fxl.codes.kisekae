namespace fxl.codes.kisekae.Entities
{
    public class PaletteDto : IKisekaeFile, IKisekaeParseable
    {
        public int Id { get; set; }
        public int KisekaeId { get; set; }
        public string Filename { get; set; }
        public string Comment { get; set; }
        public byte[] Data { get; set; }
    }
}