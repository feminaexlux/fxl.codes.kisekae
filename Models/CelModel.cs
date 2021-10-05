using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace fxl.codes.kisekae.Models
{
    public class CelModel
    {
        public const string Regex = "#([0-9]*)[\\.]?([0-9]*?)\\s([a-zA-Z\\-0-9]*\\.[cCeElL]+)\\s[\\*]?([0-9]*)?\\s?:([0-9\\s]*);(.*)";
        public Dictionary<int, string> ImageByPalette = new();

        public CelModel(string line)
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
                        Id = int.Parse(value.Value);
                        break;
                    case "2":
                        Fix = int.Parse(value.Value);
                        break;
                    case "3":
                        FileName = value.Value;
                        break;
                    case "4":
                        PaletteId = int.Parse(value.Value);
                        break;
                    case "5":
                        foreach (var id in value.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)) Sets[int.Parse(id)] = true;
                        break;
                    case "6":
                        Comment = value.Value;
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