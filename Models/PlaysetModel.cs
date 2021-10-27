using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using SixLabors.ImageSharp;

namespace fxl.codes.kisekae.Models
{
    public class PlaysetModel
    {
        public string Name { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        [JsonIgnore] public int BorderColorIndex { get; set; }
        [JsonIgnore] public Color BorderColor { get; set; } = Color.Black;
        [JsonIgnore] public List<PaletteModel> Palettes { get; } = new();
        public List<CelModel> Cels { get; } = new();

        public bool[] EnabledSets
        {
            get
            {
                var enabled = new bool[10];

                for (var index = 0; index < enabled.Length; index++) enabled[index] = Cels.Any(x => x.Sets[index]);

                return enabled;
            }
        }
    }
}