using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace fxl.codes.kisekae.Models
{
    public class CelModel
    {
        public Dictionary<int, string> ImageByPalette = new();

        public int Id { get; init; }
        public int Fix { get; init; }
        public string FileName { get; init; }
        public int PaletteId { get; init; }
        public bool[] Sets { get; } = new bool[10];
        public string Comment { get; init; }
        [JsonIgnore] public int Transparency { get; set; }
        public double Opacity { get; internal set; } = 1.0;
        public Coordinate[] InitialPositions { get; } = new Coordinate[10];
        [JsonIgnore] public string DefaultImage => ImageByPalette.ContainsKey(PaletteId) ? ImageByPalette[PaletteId] : null;
        public Coordinate Offset { get; set; }
        [JsonIgnore] public int Height { get; internal set; }
        [JsonIgnore] public int Width { get; internal set; }
        [JsonIgnore] public int ZIndex { get; internal set; }
    }
}