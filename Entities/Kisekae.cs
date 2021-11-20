using System.Collections.Generic;

namespace fxl.codes.kisekae.Entities
{
    public class Kisekae
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string CheckSum { get; set; }
        public List<Configuration> Configurations { get; set; } = new();
        public List<Cel> Cels { get; set; } = new();
        public List<Palette> Palettes { get; set; } = new();
    }
}