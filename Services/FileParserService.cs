using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using fxl.codes.kisekae.Models;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = System.Drawing.Color;

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

            if (buffer[..KissHeader.Length].SequenceEqual(KissHeader))
            {
                // Verify palette mark?
                if (buffer[4] != 16) throw new InvalidDataException("File provided is not a valid palette file");
                var colorDepth = Convert.ToInt32(buffer[5]);
                var colorsPerGroup = BitConverter.ToInt16(buffer, 8);
                var groups = BitConverter.ToInt16(buffer, 10);

                palette.Colors = GetColors(new BitArray(buffer[32..]), colorDepth, colorsPerGroup, groups);
            }
            else
            {
                palette.Colors = GetColors(new BitArray(buffer));
            }
        }

        public void ParseCel(string directory, CelModel cel, IEnumerable<PaletteModel> palettes)
        {
            _logger.LogTrace($"Reading cel {cel.FileName} from {directory}");
            var buffer = ReadToBuffer(directory, cel.FileName);

            if (buffer[..KissHeader.Length].SequenceEqual(KissHeader))
            {
                // Verify cel mark?
                if (buffer[4] != 16) throw new InvalidDataException("File provided is not a valid palette file");
                var pixelBits = Convert.ToInt32(buffer[5]);
                var width = BitConverter.ToInt16(buffer, 8);
                var height = BitConverter.ToInt16(buffer, 10);
                var xOffset = BitConverter.ToInt16(buffer, 12);
                var yOffset = BitConverter.ToInt16(buffer, 14);

                GetCelImages(new BitArray(buffer[32..]), palettes, width, height, pixelBits, xOffset, yOffset);
            }
            else
            {
                var width = BitConverter.ToInt16(buffer, 0);
                var height = BitConverter.ToInt16(buffer, 2);

                GetCelImages(new BitArray(buffer[4..]), palettes, width, height);
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

        private Color[] GetColors(BitArray bitArray, int depth = 12, int colorsPerGroup = 16, int groups = 10)
        {
            _logger.LogTrace("Converting bit array to colors");
            var colorCounter = 0;
            var colors = new Color[colorsPerGroup * groups];
            var multiplier = depth == 12 ? 16 : 1;
            var data = new bool[bitArray.Length];
            bitArray.CopyTo(data, 0);

            var length = depth / 3;
            for (var index = 0; index < colorsPerGroup * groups; index += depth)
            {
                var greenStart = index + length;
                var blueStart = index + length * 2;

                var red = GetValue(data[index..greenStart]) * multiplier;
                var green = GetValue(data[greenStart..blueStart]) * multiplier;
                var blue = GetValue(data[blueStart..(index + depth)]) * multiplier;
                colors[colorCounter] = Color.FromArgb(255, red, green, blue);
                colorCounter++;
            }

            return colors;
        }

        private void GetCelImages(BitArray bitArray, IEnumerable<PaletteModel> palettes, int width, int height, int pixelBits = 4, int xOffset = 0, int yOffset = 0)
        {
            _logger.LogTrace("Converting bit array to base64 encoded gifs per palette");
            var data = new bool[bitArray.Length];
            bitArray.CopyTo(data, 0);

            foreach (var palette in palettes)
            {
                using var bitmap = new Image<Bgr24>(width, height);
                var pixelCounter = 0;
                
                for (var row = 0; row < height; row++)
                {
                    var span = bitmap.GetPixelRowSpan(row);
                }
            }
        }

        private static int GetValue(bool[] data)
        {
            var bits = new BitArray(data);
            var value = new int[1];
            bits.CopyTo(value, 0);

            return value[0];
        }
    }
}