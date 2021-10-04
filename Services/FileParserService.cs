using System;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using fxl.codes.kisekae.Models;
using Microsoft.Extensions.Logging;

namespace fxl.codes.kisekae.Services
{
    public class FileParserService
    {
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
            using var stream = _storage.OpenFile(Path.Combine(directory, palette.FileName), FileMode.Open);
            if (!stream.CanRead) throw new InvalidDataException();

            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            var header = Encoding.ASCII.GetBytes("KiSS");
            if (buffer[..header.Length].SequenceEqual(header))
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

        private Color[] GetColors(BitArray bitArray, int depth = 12, int colorsPerGroup = 16, int groups = 10)
        {
            _logger.LogTrace($"Converting bit array to colors");
            var colorCounter = 0;
            var colors = new Color[colorsPerGroup * groups];
            var data = new bool[bitArray.Length];
            bitArray.CopyTo(data, 0);

            var length = depth / 3;
            for (var index = 0; index < colorsPerGroup * groups; index += depth)
            {
                var greenStart = index + length;
                var blueStart = index + length * 2;
                
                var red = GetValue(data[index..greenStart]);
                var green = GetValue(data[greenStart..blueStart]);
                var blue = GetValue(data[blueStart..(index + depth)]);
                colors[colorCounter] = new Color(red, green, blue, colorCounter / colorsPerGroup, depth);
                colorCounter++;
            }

            return colors;
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