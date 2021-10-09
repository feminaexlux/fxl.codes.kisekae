using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using fxl.codes.kisekae.Models;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace fxl.codes.kisekae.Services
{
    public class FileParserService
    {
        private static readonly byte[] KissHeader = Encoding.ASCII.GetBytes("KiSS");

        private readonly ILogger<FileParserService> _logger;
        private readonly IsolatedStorageFile _storage;

        public FileParserService(ILogger<FileParserService> logger)
        {
            _logger = logger;
            _storage = IsolatedStorageFile.GetUserStoreForApplication();
        }

        public void ParsePalette(string directory, PaletteModel palette)
        {
            _logger.LogTrace($"Reading palette {palette.FileName} from {directory}");
            var buffer = ReadToBuffer(directory, palette.FileName);
            if (buffer.Length == 0) return;

            if (buffer[..KissHeader.Length].SequenceEqual(KissHeader))
            {
                // Verify palette mark?
                if (buffer[4] != 16) _logger.LogError($"{palette.FileName} is not a valid palette file");

                var colorDepth = Convert.ToInt32(buffer[5]);
                var colorsPerGroup = BitConverter.ToInt16(buffer, 8);
                var groups = BitConverter.ToInt16(buffer, 10);

                palette.Colors = GetColors(buffer[32..], colorDepth, colorsPerGroup, groups);
            }
            else
            {
                palette.Colors = GetColors(buffer);
            }
        }

        public void ParseCel(string directory, CelModel cel, IEnumerable<PaletteModel> palettes)
        {
            _logger.LogTrace($"Reading cel {cel.FileName} from {directory}");
            var buffer = ReadToBuffer(directory, cel.FileName);
            if (buffer.Length == 0) return;

            if (buffer[..KissHeader.Length].SequenceEqual(KissHeader))
            {
                // Verify cel mark?
                if (buffer[4] != 32) _logger.LogError($"{cel.FileName} is not a valid cel file");

                var pixelBits = Convert.ToInt32(buffer[5]);
                var width = BitConverter.ToInt16(buffer, 8);
                var height = BitConverter.ToInt16(buffer, 10);
                var xOffset = BitConverter.ToInt16(buffer, 12);
                var yOffset = BitConverter.ToInt16(buffer, 14);

                cel.ImageByPalette = GetCelImages(buffer[32..], palettes.ToArray(), width, height, pixelBits, xOffset, yOffset);
            }
            else
            {
                var width = BitConverter.ToInt16(buffer, 0);
                var height = BitConverter.ToInt16(buffer, 2);

                cel.ImageByPalette = GetCelImages(buffer[4..], palettes.ToArray(), width, height);
            }
        }

        private byte[] ReadToBuffer(string directory, string filename)
        {
            using var stream = _storage.OpenFile(Path.Combine(directory, filename), FileMode.Open);
            if (!stream.CanRead) throw new InvalidDataException();

            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            return buffer;
        }

        private Color[] GetColors(byte[] bytes, int depth = 12, int colorsPerGroup = 16, int groups = 10)
        {
            _logger.LogTrace("Converting byte array to colors");
            var colorCounter = 0;
            var colors = new Color[colorsPerGroup * groups];
            var chunkSize = depth == 12 ? 2 : 3;

            for (var index = 0; index < bytes.Length; index += chunkSize)
            {
                colors[colorCounter] = GetColor(bytes[index..(index + chunkSize)], depth);
                colorCounter++;
            }

            return colors;
        }

        private static Color GetColor(IReadOnlyList<byte> bytes, int depth)
        {
            if (depth != 12) return Color.FromRgb(bytes[0], bytes[1], bytes[2]);

            var redBlue = bytes[0];
            var red = (redBlue >> 4) & 0x0F;
            red += red * 16;

            var blue = redBlue & 0x0F;
            blue += blue * 16;

            var zeroGreen = bytes[1];
            var green = zeroGreen & 0x0F;
            green += green * 16;

            return Color.FromRgb(Convert.ToByte(red), Convert.ToByte(green), Convert.ToByte(blue));
        }

        private Dictionary<int, string> GetCelImages(IReadOnlyList<byte> bytes,
                                                     IReadOnlyList<PaletteModel> palettes,
                                                     int width,
                                                     int height,
                                                     int pixelBits = 4,
                                                     int xOffset = 0,
                                                     int yOffset = 0)
        {
            _logger.LogTrace("Converting byte array to base64 encoded gifs per palette");
            var dictionary = new Dictionary<int, string>();
            var step = (int)Math.Ceiling((double)8 / pixelBits);

            for (var paletteIndex = 0; paletteIndex < palettes.Count; paletteIndex++)
            {
                var palette = palettes[paletteIndex];
                using var bitmap = new Image<Rgba32>(width, height);
                var index = 0;

                for (var row = 0; row < height; row++)
                {
                    var span = bitmap.GetPixelRowSpan(row);
                    for (var column = 0; column < width; column += step)
                    {
                        var pixelByte = bytes[index];
                        var nib1 = (pixelByte >> 4) & 0x0F;
                        span[column] = nib1 == 0 ? Color.Black.WithAlpha(0) : palette.Colors[nib1];

                        if (column + 1 < width)
                        {
                            var nib2 = pixelByte & 0x0F;
                            span[column + 1] = nib2 == 0 ? Color.Black.WithAlpha(0) : palette.Colors[nib2];
                        }

                        index++;
                    }
                }

                using var memory = new MemoryStream();
                bitmap.SaveAsGif(memory);
                dictionary.Add(paletteIndex, Convert.ToBase64String(memory.ToArray()));
            }

            return dictionary;
        }
    }
}