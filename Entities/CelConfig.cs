namespace fxl.codes.kisekae.Entities
{
    public class CelConfig
    {
        public int Id { get; set; }
        public Cel Cel { get; set; }
        public Configuration Configuration { get; set; }
        public int Mark { get; set; }
        public int Fix { get; set; }
        public Palette Palette { get; set; }
        public int PaletteGroup { get; set; }
        public string Comment { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Transparency { get; set; }
        public Set Sets { get; set; }
        public Render Render { get; set; }
    }
}