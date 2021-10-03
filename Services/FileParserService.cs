using System;
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
                // Matching header I guess?
            }
        }
    }
}