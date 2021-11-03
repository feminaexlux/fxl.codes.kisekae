using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace fxl.codes.kisekae.Entities
{
    [Table("kisekae")]
    public class KisekaeDto
    {
        public int Id { get; set; }
        public string Name { get; init; }
        public string Filename { get; init; }
        public string Checksum { get; set; }
        public IEnumerable<ConfigurationDto> Configurations { get; set; }
    }
}