using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using fxl.codes.kisekae.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace fxl.codes.kisekae.Services
{
    public class ConfigurationReaderService
    {
        private readonly ILogger<ConfigurationReaderService> _logger;
        private readonly FileParserService _fileParser;

        public ConfigurationReaderService(ILogger<ConfigurationReaderService> logger, FileParserService fileParser)
        {
            _logger = logger;
            _fileParser = fileParser;
        }

        public PlaysetModel ReadCnf(IFormFile file)
        {
            _logger.LogTrace($"Reading filename {file.FileName}");
            var set = ParseStream(file.OpenReadStream());
            set.Name = file.FileName;
            return set;
        }

        public PlaysetModel ParseStream(Stream fileStream, string directory = null)
        {
            var model = new PlaysetModel();
            var initialPositions = new StringBuilder();
            var borderColorIndex = 0;
            
            using var reader = new StreamReader(fileStream);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;

                switch (line.ToCharArray()[0])
                {
                    case '(':
                        var resolutionRegex = new Regex(@"\((?<Width>[0-9]*).(?<Height>[0-9]*)\)");
                        var resolutionMatch = resolutionRegex.Match(line);
                        model.Width = int.Parse(resolutionMatch.Groups["Width"].Value);
                        model.Height = int.Parse(resolutionMatch.Groups["Height"].Value);
                        break;
                    case '[':
                        var borderValue = line.Replace("[", "");
                        if (borderValue.Contains(';')) borderValue = borderValue.Split(';')[0].Trim();
                        borderColorIndex = int.Parse(borderValue);
                        break;
                    case '%':
                        model.Palettes.Add(new PaletteModel(_logger, line));
                        break;
                    case '#':
                        model.Cels.Add(new CelModel(_logger, line));
                        break;
                    case '$':
                    case ' ':
                        initialPositions.Append(line.Replace("\\r\\n", "").Replace("\\n", ""));
                        break;
                }
            }
            
            SetInitialPositions(model, initialPositions.ToString());
            if (string.IsNullOrEmpty(directory)) return model;
            
            foreach (var palette in model.Palettes)
            {
                _fileParser.ParsePalette(directory, palette);
            }

            foreach (var cel in model.Cels.Where(x => !string.IsNullOrEmpty(x.FileName)))
            {
                _fileParser.ParseCel(directory, cel, model.Palettes);
            }
            
            if (model.Palettes.Any() && model.Palettes[0].Colors.Any()) model.BorderColor = model.Palettes[0].Colors[borderColorIndex];
            model.Cels.Reverse();

            return model;
        }

        private static void SetInitialPositions(PlaysetModel model, string positions)
        {
            var sets = positions.Split('$', StringSplitOptions.RemoveEmptyEntries);
            for (var index = 0; index < sets.Length; index++)
            {
                var value = sets[index].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                model.CurrentPalettes[index] = int.Parse(value[0]);

                for (var innerIndex = 1; innerIndex < value.Length; innerIndex++)
                {
                    if (value[innerIndex] == "*") continue;
                    var point = value[innerIndex].Split(',', StringSplitOptions.RemoveEmptyEntries);
                    
                    foreach (var cel in model.Cels.Where(x => x.Id == innerIndex - 1))
                    {
                        cel.InitialPositions[index] = new Coordinate(int.Parse(point[0]), int.Parse(point[1]));
                    }
                }
            }
        }
    }
}