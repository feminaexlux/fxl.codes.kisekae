namespace fxl.codes.kisekae.data.Entities
{
    public class Palette
    {
        public int Id { get; set; }
        public Kisekae Kisekae { get; set; }
        public string FileName { get; set; }
        public List<PaletteColor> Colors { get; set; } = new();
        public byte[] Data { get; set; }
    }
}