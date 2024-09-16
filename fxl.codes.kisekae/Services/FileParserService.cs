using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using fxl.codes.kisekae.data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace fxl.codes.kisekae.Services;

public class FileParserService
{
    private static readonly byte[] KissHeader = "KiSS"u8.ToArray();
    private static readonly Color Transparent = Color.Black.WithAlpha(0);

    private readonly ILogger<FileParserService> _logger;
    private readonly IsolatedStorageFile _storage;

    public FileParserService(ILogger<FileParserService> logger)
    {
        _logger = logger;
        _storage = IsolatedStorageFile.GetUserStoreForApplication();
    }

    public async Task UnzipLzh(IFormFile file, MemoryStream memoryStream = null)
    {
        if (!_storage.FileExists(file.FileName))
        {
            using var writer = _storage.CreateFile(file.FileName);

            if (memoryStream != null)
                memoryStream.CopyTo(writer);
            else
                file.CopyTo(writer);

            writer.Flush();
            writer.Close();
        }

        var root = _storage.GetType()
            .GetProperty("RootDirectory", BindingFlags.Instance | BindingFlags.NonPublic)?
            .GetValue(_storage)?
            .ToString() ?? "";

        var directory = Path.GetFileNameWithoutExtension(file.FileName);
        if (!_storage.DirectoryExists(directory)) _storage.CreateDirectory(directory);

        var args = $"x \"{Path.Combine(root, file.FileName)}\" -o\"{Path.Combine(root, directory)}\"";
        var process = Process.Start("7z", args);
        if (process == null) throw new ArgumentException("Unable to run 7z against archive");

        await process.WaitForExitAsync();
    }

    public Color[] ParsePalette(Palette palette, out int groups, out int colorsPerGroup)
    {
        groups = 0;
        colorsPerGroup = 0;

        if (palette.Data.Length == 0) return null;
        if (!palette.Data[..KissHeader.Length].SequenceEqual(KissHeader)) return GetColors(palette.Data);

        // Verify palette mark?
        if (palette.Data[4] != 16) _logger.LogError($"{palette.FileName} is not a valid palette file");

        var colorDepth = Convert.ToInt32(palette.Data[5]);
        colorsPerGroup = BitConverter.ToInt16(palette.Data, 8);
        groups = BitConverter.ToInt16(palette.Data, 10);

        return GetColors(palette.Data[32..], colorDepth, colorsPerGroup, groups);
    }

    public void RenderCel(CelConfig celConfig)
    {
        _logger.LogTrace($"Reading cel {celConfig.Cel.FileName}");
        var buffer = celConfig.Cel?.Data?.ToArray();
        if (buffer == null || buffer.Length == 0) return;

        if (buffer[..KissHeader.Length].SequenceEqual(KissHeader))
        {
            // Verify cel mark?
            if (buffer[4] != 32) _logger.LogError($"{celConfig.Cel.FileName} is not a valid cel file");

            var pixelBits = Convert.ToInt32(buffer[5]);
            var width = BitConverter.ToInt16(buffer, 8);
            var height = BitConverter.ToInt16(buffer, 10);
            var xOffset = BitConverter.ToInt16(buffer, 12);
            var yOffset = BitConverter.ToInt16(buffer, 14);

            celConfig.Cel.OffsetX = xOffset;
            celConfig.Cel.OffsetY = yOffset;
            celConfig.Cel.Height = height;
            celConfig.Cel.Width = width;

            celConfig.Render = GetCelImage(buffer[32..], celConfig.Palette, width, height, pixelBits);
        }
        else
        {
            var width = BitConverter.ToInt16(buffer, 0);
            var height = BitConverter.ToInt16(buffer, 2);

            celConfig.Cel.Height = height;
            celConfig.Cel.Width = width;
            celConfig.Render = GetCelImage(buffer[4..], celConfig.Palette, width, height);
        }
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

    private Render GetCelImage(IReadOnlyCollection<byte> bytes,
                               Palette palette,
                               int width,
                               int height,
                               int pixelBits = 4)
    {
        _logger.LogTrace("Converting byte array to base64 encoded gifs per palette");
        var values = GetValues(bytes, pixelBits);

        using var bitmap = new Image<Rgba32>(width, height);
        bitmap.ProcessPixelRows(processor =>
        {
            var index = 0;
            for (var y = 0; y < processor.Height; y++)
            {
                var row = processor.GetRowSpan(y);
                for (var x = 0; x < row.Length; x++)
                {
                    var value = values[index];
                    var isTransparent = value == 0 || value >= palette.Colors.Count;
                    row[x] = isTransparent ? Transparent : Color.ParseHex(palette.Colors[value].Hex);
                    index++;
                }

                if (width % 2 != 0 && pixelBits == 4) index++;
            }
        });

        using var memory = new MemoryStream();
        bitmap.SaveAsGif(memory);

        return new Render
        {
            Image = memory.ToArray()
        };
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