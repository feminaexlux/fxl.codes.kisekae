using System.Text.RegularExpressions;

namespace fxl.codes.kisekae.Models
{
    public class PaletteModel
    {
        private const string Regex = "%([a-zA-Z0-9\\-]*\\.[kKcCfF]+)[\\s]*;(.*)";

        public PaletteModel(string line)
        {
            var matcher = new Regex(Regex);
            var match = matcher.Match(line);

            foreach (var group in matcher.GetGroupNames())
            {
                if (group == "0") continue;

                match.Groups.TryGetValue(group, out var value);
                if (string.IsNullOrEmpty(value?.Value)) continue;

                switch (group)
                {
                    case "1":
                        FileName = value.Value;
                        break;
                    case "2":
                        Comment = value.Value;
                        break;
                }
            }
        }

        public string FileName { get; }
        public string Comment { get; }
        public Color[] Colors { get; set; }
    }

    public class Color
    {
        public Color(int red, int green, int blue, int group, int depth)
        {
            var multiplier = depth == 12 ? 16 : 1;
            Red = red * multiplier;
            Green = green * multiplier;
            Blue = blue * multiplier;
            Group = group;
        }

        public int Red { get; }
        public int Green { get; }
        public int Blue { get; }
        public int Group { get; }
    }
}