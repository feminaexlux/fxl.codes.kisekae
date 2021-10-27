using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace fxl.codes.kisekae.Entities
{
    [Table("kisekae")]
    public class KisekaeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Filename { get; set; }
        public string Checksum { get; set; }
        public IEnumerable<ConfigurationDto> Configurations { get; set; }
    }
}