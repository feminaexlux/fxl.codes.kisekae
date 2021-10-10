using System;
using fxl.codes.kisekae.Services;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace fxl.codes.kisekae.Models
{
    public class PaletteModel
    {
        internal PaletteModel(ILogger<ConfigurationReaderService> logger, string line)
        {
            logger.LogTrace($"Parsing palette line {line}");
            
            var parts = line.Split(';');
            FileName = parts[0].Replace("%", "").Trim();

            if (parts.Length > 1) Comment = parts[1].Trim();
        }

        public string FileName { get; }
        public string Comment { get; }
        public Color[] Colors { get; set; } = Array.Empty<Color>();
    }
}