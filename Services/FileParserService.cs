using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Text;
using fxl.codes.kisekae.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace fxl.codes.kisekae.Services
{
    public class FileParserService
    {
        private static readonly byte[] KissHeader = Encoding.ASCII.GetBytes("KiSS");
        private static readonly Color Transparent = Color.Black.WithAlpha(0);

        private readonly ILogger<FileParserService> _logger;
        private readonly IsolatedStorageFile _storage;

        public FileParserService(ILogger<FileParserService> logger)
        {
            _logger = logger;
            _storage = IsolatedStorageFile.GetUserStoreForApplication();
        }

        public void UnzipLzh(IFormFile file)
        {
            if (_storage.FileExists(file.FileName)) _storage.DeleteFile(file.FileName);
            
            using var writer = _storage.CreateFile(file.FileName);
            file.CopyTo(writer);
            writer.Flush();
            writer.Close();

            var root = _storage.GetType()
                .GetProperty("RootDirectory", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(_storage)
                .ToString() ?? "";

            var directory = Path.GetFileNameWithoutExtension(file.FileName);
            if (_storage.DirectoryExists(directory)) _storage.DeleteDirectory(directory);
            _storage.CreateDirectory(directory);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "7z",
                    Arguments = $"x {Path.Combine(root, file.FileName)} -o{Path.Combine(root, directory)}",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };

            if (!process.Start()) throw new InvalidDataException("Unable to call p7zip to extract archive");
            
            while (!process.StandardOutput.EndOfStream) _logger.LogTrace(process.StandardOutput.ReadLine());
            process.WaitForExit();
            
            _storage.DeleteFile(file.FileName);
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

                cel.ImageByPalette = GetCelImages(buffer[32..], palettes.ToArray(), width, height, pixelBits);
                cel.Offset = new Coordinate(xOffset, yOffset);
                cel.Height = height;
                cel.Width = width;
            }
            else
            {
                var width = BitConverter.ToInt16(buffer, 0);
                var height = BitConverter.ToInt16(buffer, 2);

                cel.ImageByPalette = GetCelImages(buffer[4..], palettes.ToArray(), width, height);
                cel.Height = height;
                cel.Width = width;
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

        private Color[] GetColors(IReadOnlyCollection<byte> bytes, int depth = 12, int colorsPerGroup = 16, int groups = 10)
        {
            _logger.LogTrace("Converting byte array to colors");
            var colorValues = GetValues(bytes, depth / 3);
            var colors = new Color[colorsPerGroup * groups];
            var length = depth == 12 ? 4 : 3;
            var multiplier = depth == 12 ? 16 : 1;
            // Nibbles order for 12 bits: R, B, 0, G

            for (var groupIndex = 0; groupIndex < groups; groupIndex++)
            for (var colorIndex = 0; colorIndex < colorsPerGroup; colorIndex++)
            {
                var start = (groupIndex + colorIndex) * length;
                var red = colorValues[start] * multiplier;
                var green = colorValues[start + (depth == 12 ? 3 : 1)] * multiplier;
                var blue = colorValues[start + (depth == 12 ? 1 : 2)] * multiplier;
                colors[groupIndex + colorIndex] = Color.FromRgb(Convert.ToByte(red), Convert.ToByte(green), Convert.ToByte(blue));
            }

            return colors;
        }

        private Dictionary<int, string> GetCelImages(IReadOnlyCollection<byte> bytes,
                                                     IReadOnlyList<PaletteModel> palettes,
                                                     int width,
                                                     int height,
                                                     int pixelBits = 4)
        {
            _logger.LogTrace("Converting byte array to base64 encoded gifs per palette");
            var dictionary = new Dictionary<int, string>();
            var values = GetValues(bytes, pixelBits);

            for (var paletteIndex = 0; paletteIndex < palettes.Count; paletteIndex++)
            {
                var palette = palettes[paletteIndex];
                using var bitmap = new Image<Rgba32>(width, height);
                var index = 0;

                for (var row = 0; row < height; row++)
                {
                    var span = bitmap.GetPixelRowSpan(row);
                    for (var column = 0; column < width; column++)
                    {
                        var value = values[index];
                        span[column] = value == 0 || value >= palette.Colors.Length ? Transparent : palette.Colors[value];
                        index++;
                    }

                    if (width % 2 != 0 && pixelBits == 4) index++;
                }

                using var memory = new MemoryStream();
                bitmap.SaveAsGif(memory);
                dictionary.Add(paletteIndex, Convert.ToBase64String(memory.ToArray()));
            }

            return dictionary;
        }

        private static int[] GetValues(IReadOnlyCollection<byte> bytes, int bits = 4)
        {
            if (bits != 4) return bytes.Select(Convert.ToInt32).ToArray();

            var values = new int[bytes.Count * 2];
            var index = 0;
            foreach (var slice in bytes)
            {
                var nib1 = (slice >> 4) & 0x0F;
                var nib2 = slice & 0x0F;
                values[index] = nib1;
                values[index + 1] = nib2;
                index += 2;
            }

            return values;
        }
    }
}