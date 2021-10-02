using System.IO;
using System.Text.RegularExpressions;
using fxl.codes.kisekae.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace fxl.codes.kisekae.Services
{
    public class ConfigurationReaderService
    {
        private readonly ILogger<ConfigurationReaderService> _logger;

        public ConfigurationReaderService(ILogger<ConfigurationReaderService> logger)
        {
            _logger = logger;
        }

        public ConfigurationModel ReadCnf(IFormFile file)
        {
            _logger.LogTrace($"Reading filename {file.FileName}");

            var model = new ConfigurationModel("");

            using var reader = new StreamReader(file.OpenReadStream());
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;

                switch (line.ToCharArray()[0])
                {
                    case '(':
                        var resolutionRegex = new Regex("\\(([0-9]*),([0-9]*)\\)");
                        var resolutionMatch = resolutionRegex.Match(line);
                        model.Width = int.Parse(resolutionMatch.Groups[1].Value);
                        model.Height = int.Parse(resolutionMatch.Groups[2].Value);
                        break;
                    case '[':
                        model.BorderColorIndex = int.Parse(line.Replace("[", ""));
                        break;
                    case '%':
                        model.Palettes.Add(new PaletteModel(line));
                        break;
                    case '#':
                        model.Cels.Add(new CelModel(line));
                        break;
                }
            }

            return model;
        }
    }
}