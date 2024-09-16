using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using fxl.codes.kisekae.data.Entities;
using Microsoft.Extensions.Logging;

namespace fxl.codes.kisekae.Services;

public class ConfigurationReaderService
{
    private const string CelRegex = @"#(?<Mark>\d*)\.?(?<Fix>\d*)\s*(?<FileName>[\w\d\-]*\.[cCeElL]*)\s*\*?"
                                    + @"(?<PaletteIndex>\d*)?\s*\:?(?<Sets>[\d\s]*)?;?(?<Comment>[\w\d\-\s\%]*)";

    private const string ResolutionRegexPattern = @"\((?<Width>[0-9]*).(?<Height>[0-9]*)\)";

    private readonly ILogger<ConfigurationReaderService> _logger;

    public ConfigurationReaderService(ILogger<ConfigurationReaderService> logger)
    {
        _logger = logger;
    }

    public void ReadConfiguration(Configuration dto,
                                  IDictionary<Configuration, int> backgroundColors,
                                  IDictionary<string, Cel> cels,
                                  Dictionary<string, Palette> palettes)
    {
        _logger.LogInformation($"Reading {dto.Name}");
        var initialPositions = new StringBuilder();
        var backgroundColorIndex = 0;
        var paletteOrder = new List<Palette>();
        var celLines = new List<string>();

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
                    backgroundColorIndex = int.Parse(borderValue);
                    break;
                case '%':
                    SetPaletteOrder(line, palettes, paletteOrder);
                    break;
                case '#':
                    celLines.Add(line);
                    break;
                case '$':
                case ' ':
                    initialPositions.Append(line);
                    break;
            }

        foreach (var line in celLines) dto.Cels.Add(SetCelConfig(line, dto, cels, paletteOrder));
        SetInitialPositions(dto, initialPositions.ToString());
        backgroundColors.Add(dto, backgroundColorIndex);
    }

    private static CelConfig SetCelConfig(string line, Configuration configuration, IDictionary<string, Cel> cels, IReadOnlyList<Palette> palettes)
    {
        var cel = new CelConfig
        {
            Configuration = configuration
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
                cel.Cel = cels[group.Value.ToLowerInvariant()];
                continue;
            }

            if (string.Equals(groupName, "PaletteIndex"))
            {
                cel.Palette = palettes[int.Parse(group.Value)];
                continue;
            }

            var property = typeof(CelConfig).GetProperty(groupName);
            if (property == null) continue;

            var value = group.Value.Trim();
            if (property.PropertyType == typeof(string)) property.SetValue(cel, value);
            if (property.PropertyType == typeof(int)) property.SetValue(cel, int.Parse(value));

            if (property.PropertyType != cel.Sets.GetType()) continue;

            foreach (var id in value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                var success = int.TryParse(id, out var result);
                if (!success) continue;
                cel.Sets[result] = true;
            }
        }

        cel.Palette ??= palettes[0];
        if (!cel.Sets.Max()) Array.Fill(cel.Sets, true);

        return cel;
    }

    private static void SetPaletteOrder(string line, IReadOnlyDictionary<string, Palette> palettes, ICollection<Palette> paletteOrder)
    {
        var parts = line.Split(';');
        var palette = palettes[parts[0].Trim().ToLowerInvariant().Replace("%", "")];
        paletteOrder.Add(palette);
    }

    private static void SetInitialPositions(Configuration dto, string positions)
    {
        positions = Regex.Replace(positions, "[\\s]+", "|");
        var sets = positions.Split('$', StringSplitOptions.RemoveEmptyEntries);
        for (var index = 0; index < sets.Length; index++)
        {
            var set = sets[index];
            var value = set.Split('|', StringSplitOptions.RemoveEmptyEntries);
            var paletteGroup = int.Parse(value[0]);

            for (var innerIndex = 1; innerIndex < value.Length; innerIndex++)
            {
                if (value[innerIndex].Contains('*')) continue;
                var point = value[innerIndex].Trim().Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var cel in dto.Cels.Where(x => x.Mark == innerIndex - 1))
                {
                    cel.PaletteGroup = paletteGroup;

                    if (!cel.Sets[index]) continue;
                    cel.Positions.Add(new CelPosition
                    {
                        Set = index,
                        X = int.Parse(point[0]),
                        Y = int.Parse(point[1])
                    });
                }
            }
        }
    }
}