namespace fxl.codes.kisekae.Entities
{
    public class CelDto : IKisekaeFile, IKisekaeParseable
    {
        public int Id { get; set; }
        public int KisekaeId { get; set; }
        public string Filename { get; set; }
        public byte[] Data { get; set; }
    }
}