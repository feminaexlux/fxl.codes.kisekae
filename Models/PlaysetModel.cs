using System.Collections.Generic;
using SixLabors.ImageSharp;

namespace fxl.codes.kisekae.Models
{
    public class PlaysetModel
    {
        public int[] CurrentPalettes = new int[10];
        public int Height { get; set; }
        public int Width { get; set; }
        public Color BorderColor { get; set; }

        public List<PaletteModel> Palettes { get; } = new();
        public List<CelModel> Cels { get; } = new();
    }
}