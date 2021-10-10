using System.Collections.Generic;
using SixLabors.ImageSharp;

namespace fxl.codes.kisekae.Models
{
    public class PlaysetModel
    {
        public int[] CurrentPalettes = new int[10];
        public string Name { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public Color BorderColor { get; set; } = Color.Black;

        public List<PaletteModel> Palettes { get; } = new();
        public List<CelModel> Cels { get; } = new();

        public bool[] EnabledSets
        {
            get
            {
                var enabled = new bool[10];
                foreach (var cel in Cels)
                    for (var index = 0; index < cel.Sets.Length; index++)
                        if (cel.Sets[index])
                            enabled[index] = true;

                return enabled;
            }
        }
    }
}