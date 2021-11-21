using System;
using System.Linq;
using System.Text.Json.Serialization;
using fxl.codes.kisekae.Entities;

namespace fxl.codes.kisekae.Models
{
    public class PlaysetModel
    {
        [JsonIgnore] public readonly string BorderColor;

        internal PlaysetModel(Configuration configuration)
        {
            BorderColor = configuration.BackgroundColorHex;

            Name = configuration.Name;
            Height = configuration.Height;
            Width = configuration.Width;
            var totalCels = configuration.Cels.Count;
            Cels = configuration.Cels.Select((x, index) => new CelModel(x, (totalCels - index) * 10)).ToArray();

            var summation = configuration.Cels.Aggregate(Set.None, (current, cel) => current | cel.Sets);
            if (summation == Set.None)
            {
                Array.Fill(Sets, true);
                return;
            }

            foreach (var value in Enum.GetValues<Set>().Where(x => (x & summation) == x && x != Set.None))
                if (value == Set.Zero) Sets[0] = true;
                else Sets[(int)Math.Log2((double)value)] = true;
        }

        public CelModel[] Cels { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
        public bool[] Sets { get; set; } = new bool[10];
        public int Width { get; set; }
    }
}