using System.Collections.Generic;

namespace fxl.codes.kisekae.Models
{
    public class PlaysetModel
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int BorderColorIndex { get; set; }

        public List<PaletteModel> Palettes { get; } = new();
        public List<CelModel> Cels { get; } = new();
        public int[] CurrentPalettes = new int[10];
    }
}