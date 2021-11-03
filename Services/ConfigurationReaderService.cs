using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using fxl.codes.kisekae.Entities;
using fxl.codes.kisekae.Models;
using Microsoft.Extensions.Logging;

namespace fxl.codes.kisekae.Services
{
    public class ConfigurationReaderService
    {
        private const string CelRegex = @"#(?<Group>\d*)\.?(?<Fix>\d*)\s*(?<FileName>[\w\d\-]*\.[cCeElL]*)\s*\*?"
                                        + @"(?<PaletteId>\d*)?\s*\:?(?<Sets>[\d\s]*)?;?(?<Comment>[\w\d\-\s\%]*)";

        private const string ResolutionRegexPattern = @"\((?<Width>[0-9]*).(?<Height>[0-9]*)\)";

        private readonly ILogger<ConfigurationReaderService> _logger;

        public ConfigurationReaderService(ILogger<ConfigurationReaderService> logger)
        {
            _logger = logger;
        }

        public void ReadConfigurationToDto(ConfigurationDto dto, IDictionary<string, CelDto> cels, IDictionary<string, PaletteDto> palettes)
        {
            var initialPositions = new StringBuilder();

            foreach (var line in dto.Data.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
                switch (line.ToCharArray()[0])
                {
                    case '(':
                        var resolutionRegex = new Regex(ResolutionRegexPattern);
                        var resolutionMatch = resolutionRegex.Match(line);
                        dto.Width = int.Parse(resolutionMatch.Groups["Width"].Value);
                        dto.Height = int.Parse(resolutionMatch.Groups["Height"].Value);
                        break;
                    case '[':
                        var borderValue = line[1..];
                        if (borderValue.Contains(';')) borderValue = borderValue.Split(';')[0].Trim();
                        dto.BorderIndex = int.Parse(borderValue);
                        break;
                    case '%':
                        dto.Palettes.Add(SetPalette(line, palettes));
                        break;
                    case '#':
                        dto.Cels.Add(SetCel(line, dto, cels));
                        break;
                    case '$':
                    case ' ':
                        initialPositions.Append(line.Replace("\\r\\n", "").Replace("\\n", ""));
                        break;
                }

            SetInitialPositions(dto, initialPositions.ToString());
        }

        private CelConfigDto SetCel(string line, IKisekaeFile configuration, IDictionary<string, CelDto> cels)
        {
            var cel = new CelConfigDto
            {
                ConfigId = configuration.Id
            };

            if (line.Contains("%t"))
            {
                var opacity = line[line.IndexOf("%t", StringComparison.InvariantCultureIgnoreCase)..].Trim();
                opacity = opacity.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[0];
                opacity = opacity.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[0];
                opacity = opacity.Replace("%t", "");
                cel.Transparency = int.Parse(opacity);
            }

            var matcher = new Regex(CelRegex);
            var match = matcher.Match(line);

            foreach (var groupName in matcher.GetGroupNames())
            {
                match.Groups.TryGetValue(groupName, out var group);
                if (string.IsNullOrEmpty(group?.Value)) continue;
                
                if (string.Equals(groupName, "FileName"))
                {
                    cel.CelId = cels[group.Value.ToLowerInvariant()].Id;
                    continue;
                }

                var property = typeof(CelConfigDto).GetProperty(groupName);
                if (property == null) continue;

                var value = group.Value.Trim();
                if (property.PropertyType == typeof(string)) property.SetValue(cel, value);
                if (property.PropertyType == typeof(int)) property.SetValue(cel, int.Parse(value));

                if (property.PropertyType != cel.Sets.GetType()) continue;

                foreach (var id in value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                {
                    var success = int.TryParse(id, out var result);
                    if (!success) continue;

                    if (cel.Sets == Set.Unset)
                        cel.Sets = (Set)(2 ^ result);
                    else
                        cel.Sets |= (Set)(2 ^ result);
                }
            }

            return cel;
        }

        private static PaletteDto SetPalette(string line, IDictionary<string, PaletteDto> palettes)
        {
            var parts = line.Split(';');
            var palette = palettes[parts[0].Trim().ToLowerInvariant().Replace("%", "")];
            if (parts.Length > 1) palette.Comment = parts[1].Trim();
            return palette;
        }

        private static void SetInitialPositions(ConfigurationDto dto, string positions)
        {
            var sets = positions.Split('$', StringSplitOptions.RemoveEmptyEntries);
            foreach (var set in sets)
            {
                var value = set.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (var innerIndex = 1; innerIndex < value.Length; innerIndex++)
                {
                    if (value[innerIndex] == "*") continue;
                    var point = value[innerIndex].Split(',', StringSplitOptions.RemoveEmptyEntries);

                    foreach (var cel in dto.Cels.Where(x => x.Id == innerIndex - 1))
                    {
                        cel.X = int.Parse(point[0]);
                        cel.Y = int.Parse(point[1]);
                    }
                }
            }
        }
    }
}