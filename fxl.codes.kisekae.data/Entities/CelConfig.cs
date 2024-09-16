namespace fxl.codes.kisekae.data.Entities
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
        public int Transparency { get; set; }
        public Render Render { get; set; }
        public List<CelPosition> Positions { get; set; } = new();
        public bool[] Sets { get; set; } = new bool[10];
    }
}