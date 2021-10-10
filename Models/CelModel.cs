using System;
using System.Collections.Generic;
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
            var matcher = new Regex(Regex);
            var match = matcher.Match(line);

            foreach (var groupName in matcher.GetGroupNames())
            {
                var property = typeof(CelModel).GetProperty(groupName);
                if (property == null) continue;
                match.Groups.TryGetValue(groupName, out var group);
                if (string.IsNullOrEmpty(group?.Value))
                {
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
        public Coordinate[] InitialPositions { get; } = new Coordinate[10];
        public string DefaultImage => ImageByPalette.ContainsKey(PaletteId) ? ImageByPalette[PaletteId] : null;
        public Coordinate Offset { get; set; }

        public Coordinate PositionForSet(int set)
        {
            var current = InitialPositions[set];
            return current == null ? new Coordinate(0, 0) : new Coordinate(current.X + Offset.X, current.Y + Offset.Y);
        }
    }
}