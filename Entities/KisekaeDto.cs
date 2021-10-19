using System.Collections.Generic;

namespace fxl.codes.kisekae.Entities
{
    public class KisekaeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Filename { get; set; }
        public string Checksum { get; set; }
        
        public IEnumerable<CelDto> Cels { get; set; }
    }
}