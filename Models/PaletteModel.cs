using System;
using SixLabors.ImageSharp;

namespace fxl.codes.kisekae.Models
{
    public class PaletteModel
    {
        public string FileName { get; internal set; }
        public string Comment { get; internal set; }
        public Color[] Colors { get; internal set; } = Array.Empty<Color>();
    }
}