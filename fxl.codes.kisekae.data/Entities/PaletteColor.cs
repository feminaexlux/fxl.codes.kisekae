namespace fxl.codes.kisekae.data.Entities
{
    public class PaletteColor
    {
        public int Id { get; set; }
        public Palette Palette { get; set; }
        public int Group { get; set; }
        public string Hex { get; set; }
    }
}