using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace fxl.codes.kisekae.Entities
{
    [Table("configuration")]
    public class ConfigurationDto : IKisekaeFile
    {
        public int Id { get; set; }
        public int KisekaeId { get; set; }
        public string Filename { get; set; }
        public string Data { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }
        public int? BorderIndex { get; set; }
        public IEnumerable<CelConfigDto> Cels { get; set; }
    }
}