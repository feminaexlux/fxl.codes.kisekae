using System.Collections.Generic;
using System.Text.Json.Serialization;
using SixLabors.ImageSharp;

namespace fxl.codes.kisekae.Models
{
    public class PlaysetModel
    {
        [JsonIgnore] public Color BorderColor = Color.Black;
        public string Name { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        [JsonIgnore] public List<PaletteModel> Palettes { get; } = new();
        public List<CelModel> Cels { get; } = new();
        public bool[] EnabledSets { get; } = new bool[10];
    }
}