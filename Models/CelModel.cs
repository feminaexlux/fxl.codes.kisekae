using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace fxl.codes.kisekae.Models
{
    public class CelModel
    {
        public const string Regex = @"#(\d*)\.?(\d*)\s*([\w\d\-]*\.[cCeElL]*)\s*\*?(\d*)?\s*\:?([\d\s]*)?;?([\w\d\-\s\%]*)";
        public Dictionary<int, string> ImageByPalette = new();

        public CelModel(string line)
        {
            var matcher = new Regex(Regex);
            var match = matcher.Match(line);

            foreach (var groupName in matcher.GetGroupNames())
            {
                if (groupName == "0") continue;

                match.Groups.TryGetValue(groupName, out var group);
                if (string.IsNullOrEmpty(group?.Value)) continue;
                var value = group.Value.Trim();

                switch (groupName)
                {
                    case "1":
                        Id = int.Parse(value);
                        break;
                    case "2":
                        Fix = int.Parse(value);
                        break;
                    case "3":
                        FileName = value;
                        break;
                    case "4":
                        PaletteId = int.Parse(value);
                        break;
                    case "5":
                        foreach (var id in value.Split(' ', StringSplitOptions.RemoveEmptyEntries)) Sets[int.Parse(id)] = true;
                        break;
                    case "6":
                        Comment = value;
                        break;
                }
            }
        }

        public int Id { get; }
        public int Fix { get; }
        public string FileName { get; }
        public int PaletteId { get; }
        public bool[] Sets { get; } = new bool[10];
        public string Comment { get; }
        public Point[] InitialPositions { get; } = new Point[10];
    }
}