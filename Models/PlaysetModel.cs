using System.Collections.Generic;
using System.Text.Json.Serialization;
using SixLabors.ImageSharp;
using Configuration = fxl.codes.kisekae.Entities.Configuration;

namespace fxl.codes.kisekae.Models
{
    public class PlaysetModel
    {
        public readonly int Height;
        public readonly string Name;
        public readonly int Width;
        public readonly CelModel[] Cels;

        [JsonIgnore] public Color BorderColor = Color.Black;

        internal PlaysetModel(Configuration configuration)
        {
            Name = configuration.Name;
            Height = configuration.Height;
            Width = configuration.Width;
            Cels = new CelModel[0];
        }
    }
}