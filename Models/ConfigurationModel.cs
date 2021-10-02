using System.Collections.Generic;

namespace fxl.codes.kisekae.Models
{
    public class ConfigurationModel
    {
        private readonly string _originalDirectory;

        public ConfigurationModel(string originalDirectory)
        {
            _originalDirectory = originalDirectory;
        }
        
        public int Height { get; set; }
        public int Width { get; set; }
        public int BorderColorIndex { get; set; }

        public List<PaletteModel> Palettes { get; } = new();
        public List<CelModel> Cels { get; } = new();
    }
}