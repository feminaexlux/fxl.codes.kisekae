using System.Collections.Generic;

namespace fxl.codes.kisekae.Models
{
    public class DirectoryModel
    {
        public string Name { get; }

        public DirectoryModel(string name)
        {
            Name = name;
        }
        
        public List<ConfigurationModel> Configurations { get; } = new();
    }

    public class ConfigurationModel
    {
        public string Name { get; }

        public ConfigurationModel(string name)
        {
            Name = name;
        }
    }
}