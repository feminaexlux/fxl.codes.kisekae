using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using fxl.codes.kisekae.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace fxl.codes.kisekae.Services
{
    public class DatabaseService
    {
        private readonly ConfigurationReaderService _configurationReaderService;
        private readonly IDbContextFactory<KisekaeContext> _contextFactory;

        private readonly FileParserService _fileParserService;
        private readonly ILogger<DatabaseService> _logger;
        private readonly IsolatedStorageFile _storage;

        public DatabaseService(ILogger<DatabaseService> logger,
                               ConfigurationReaderService configurationReaderService,
                               FileParserService fileParserService,
                               IDbContextFactory<KisekaeContext> contextFactory)
        {
            _logger = logger;
            _configurationReaderService = configurationReaderService;
            _fileParserService = fileParserService;
            _contextFactory = contextFactory;
            _storage = IsolatedStorageFile.GetUserStoreForApplication();
        }

        public IEnumerable<Kisekae> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return context.KisekaeSets
                .Include(x => x.Configurations)
                .ToArray();
        }

        public Configuration GetConfig(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Configurations
                .Include(x => x.Cels).ThenInclude(x => x.Render)
                .Include(x => x.Kisekae).ThenInclude(x => x.Palettes).ThenInclude(x => x.Colors)
                .AsSplitQuery()
                .FirstOrDefault(x => x.Id == id);
        }

        public async void StoreToDatabase(IFormFile file)
        {
            var memoryStream = await GetAsMemoryStream(file);
            var checksum = Convert.ToBase64String(await SHA256.Create().ComputeHashAsync(memoryStream));
            memoryStream.Position = 0; // Reset for re-read

            await using var context = await _contextFactory.CreateDbContextAsync();
            var existing = await context.KisekaeSets
                .FirstOrDefaultAsync(x => string.Equals(x.FileName, file.FileName)
                                          || string.Equals(x.CheckSum, checksum));
            if (existing != null) return;

            _fileParserService.UnzipLzh(file, memoryStream);
            var directory = Path.GetFileNameWithoutExtension(file.FileName);

            var kisekae = new Kisekae
            {
                FileName = file.FileName,
                CheckSum = checksum
            };

            var filenames = _storage.GetFileNames($"{Path.Combine(directory, "*")}");
            await SetInnerFiles(kisekae, directory, filenames);

            foreach (var config in kisekae.Configurations)
                _configurationReaderService.ReadConfigurationToDto(config,
                    kisekae.Cels.ToDictionary(x => x.FileName.ToLowerInvariant()),
                    kisekae.Palettes.ToDictionary(x => x.FileName.ToLowerInvariant()));

            SetPaletteColors(kisekae.Palettes);

            foreach (var celConfig in kisekae.Configurations.SelectMany(config => config.Cels))
            {
                _fileParserService.RenderCel(celConfig);
            }

            await context.AddAsync(kisekae);
            await context.SaveChangesAsync();
        }

        private async Task SetInnerFiles(Kisekae kisekae, string directory, IEnumerable<string> filenames)
        {
            foreach (var filename in filenames)
            {
                await using var reader = _storage.OpenFile(Path.Combine(directory, filename), FileMode.Open);
                if (!reader.CanRead) throw new InvalidDataException();

                var bytes = new byte[reader.Length];
                reader.Read(bytes, 0, bytes.Length);

                switch (Path.GetExtension(filename).ToLower())
                {
                    case ".cel":
                        kisekae.Cels.Add(new Cel
                        {
                            FileName = filename,
                            Data = bytes
                        });
                        break;
                    case ".cnf":
                        kisekae.Configurations.Add(new Configuration
                        {
                            Name = filename,
                            Data = Encoding.ASCII.GetString(bytes)
                        });
                        break;
                    case ".kcf":
                        kisekae.Palettes.Add(new Palette
                        {
                            FileName = filename,
                            Data = bytes
                        });
                        break;
                }
            }
        }

        private void SetPaletteColors(IEnumerable<Palette> palettes)
        {
            foreach (var palette in palettes)
            {
                var colors = _fileParserService.ParsePalette(palette, out var groups, out var colorsPerGroup);

                for (var groupIndex = 0; groupIndex < groups; groupIndex++)
                for (var colorIndex = 0; colorIndex < colorsPerGroup; colorIndex++)
                {
                    var color = colors[groupIndex * colorsPerGroup + colorIndex];
                    palette.Colors.Add(new PaletteColor
                    {
                        Group = groupIndex,
                        Hex = color.ToHex()
                    });
                }
            }
        }

        private static async Task<MemoryStream> GetAsMemoryStream(IFormFile file)
        {
            var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            return stream;
        }
    }
}