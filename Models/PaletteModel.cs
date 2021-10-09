using System;
using SixLabors.ImageSharp;

namespace fxl.codes.kisekae.Models
{
    public class PaletteModel
    {
        public PaletteModel(string line)
        {
            var parts = line.Split(';');
            FileName = parts[0].Replace("%", "").Trim();

            if (parts.Length > 1) Comment = parts[1].Trim();
        }

        public string FileName { get; }
        public string Comment { get; }
        public Color[] Colors { get; set; } = Array.Empty<Color>();
    }
}