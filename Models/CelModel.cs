using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using fxl.codes.kisekae.Services;
using Microsoft.Extensions.Logging;

namespace fxl.codes.kisekae.Models
{
    public class CelModel
    {
        private const string Regex = @"#(?<Id>\d*)\.?(?<Fix>\d*)\s*(?<FileName>[\w\d\-]*\.[cCeElL]*)\s*\*?(?<PaletteId>\d*)?\s*\:?(?<Sets>[\d\s]*)?;?(?<Comment>[\w\d\-\s\%]*)";
        public Dictionary<int, string> ImageByPalette = new();

        internal CelModel(ILogger<ConfigurationReaderService> logger, string line)
        {
            logger.LogTrace($"Parsing cel line {line}");

            if (line.Contains("%t"))
            {
                var opacity = line[line.IndexOf("%t")..].Trim();
                opacity = opacity.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[0];
                opacity = opacity.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[0];
                opacity = opacity.Replace("%t", "");
                Opacity = 1.0 - double.Parse(opacity) / 255;
            }
            
            var matcher = new Regex(Regex);
            var match = matcher.Match(line);

            foreach (var groupName in matcher.GetGroupNames())
            {
                var property = typeof(CelModel).GetProperty(groupName);
                if (property == null) continue;
                match.Groups.TryGetValue(groupName, out var group);
                if (string.IsNullOrEmpty(group?.Value))
                {
                    if (string.Equals(groupName, "Sets"))
                        for (var index = 0; index < Sets.Length; index++)
                            Sets[index] = true;

                    logger.LogWarning($"Unable to find regex group {groupName} in {line}");
                    continue;
                }

                var value = group.Value.Trim();
                if (property.PropertyType == typeof(string)) property.SetValue(this, value);
                if (property.PropertyType == typeof(int)) property.SetValue(this, int.Parse(value));

                if (property.PropertyType != Sets.GetType()) continue;

                foreach (var id in value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                {
                    var success = int.TryParse(id, out var result);
                    if (success) Sets[result] = true;
                    else logger.LogWarning($"Unable to parse {id} as cel sets");
                }
            }
        }

        public int Id { get; init; }
        public int Fix { get; init; }
        public string FileName { get; init; }
        public int PaletteId { get; init; }
        public bool[] Sets { get; } = new bool[10];
        public string Comment { get; init; }
        public double Opacity { get; } = 1.0;
        public Coordinate[] InitialPositions { get; } = new Coordinate[10];
        [JsonIgnore] public string DefaultImage => ImageByPalette.ContainsKey(PaletteId) ? ImageByPalette[PaletteId] : null;
        public Coordinate Offset { get; set; }
        [JsonIgnore] public int Height { get; internal set; }
        [JsonIgnore] public int Width { get; internal set; }
        [JsonIgnore] public int ZIndex { get; internal set; }
    }
}