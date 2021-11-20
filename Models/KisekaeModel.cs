using System.Collections.Generic;
using System.Linq;
using fxl.codes.kisekae.Entities;

namespace fxl.codes.kisekae.Models
{
    public class KisekaeModel
    {
        public readonly IEnumerable<KeyValuePair<int, string>> Configurations;
        public readonly string Name;

        internal KisekaeModel(Kisekae kisekae)
        {
            Name = kisekae.FileName;
            Configurations = kisekae.Configurations.Select(x => new KeyValuePair<int, string>(x.Id, x.Name)).ToArray();
        }
    }
}