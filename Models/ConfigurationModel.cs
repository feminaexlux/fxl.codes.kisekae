using System.Collections.Generic;
using fxl.codes.kisekae.Entities;

namespace fxl.codes.kisekae.Models
{
    public class ConfigurationModel
    {
        public ConfigurationModel(KisekaeDto dto)
        {
            Id = dto.Id;
            Name = dto.Name;

            Configurations = new Dictionary<int, string>();
            foreach (var config in dto.Configurations) Configurations.Add(config.Id, config.Filename);
        }

        public int Id { get; }
        public string Name { get; }
        public IDictionary<int, string> Configurations { get; }
    }
}