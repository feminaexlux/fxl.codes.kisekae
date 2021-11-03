using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;
using fxl.codes.kisekae.Entities;
using fxl.codes.kisekae.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace fxl.codes.kisekae.Services
{
    public class DatabaseService
    {
        private const string ResolutionRegexPattern = @"\((?<Width>[0-9]*).(?<Height>[0-9]*)\)";
        private readonly ConfigurationReaderService _configurationReaderService;

        private readonly string _connectionString;
        private readonly FileParserService _fileParserService;
        private readonly ILogger<DatabaseService> _logger;
        private readonly IsolatedStorageFile _storage;

        public DatabaseService(ILogger<DatabaseService> logger,
                               IConfiguration configuration,
                               ConfigurationReaderService configurationReaderService,
                               FileParserService fileParserService)
        {
            _logger = logger;
            _configurationReaderService = configurationReaderService;
            _fileParserService = fileParserService;
            _connectionString = configuration.GetConnectionString("kisekae");
            _storage = IsolatedStorageFile.GetUserStoreForApplication();
        }

        public async Task<IEnumerable<KisekaeDto>> GetAll()
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var multi = await connection.QueryMultipleAsync("select * from kisekae; select * from configuration");
            var files = multi.Read<KisekaeDto>();
            var configs = multi.Read<ConfigurationDto>();
            var dictionary = configs.ToLookup(x => x.KisekaeId);
            await connection.CloseAsync();

            foreach (var kiss in files) kiss.Configurations = dictionary[kiss.Id];
            return files;
        }

        public async void GetKisekaeConfig(int id, int configId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var queryParams = new { Id = id, ConfigId = configId };
            var cels = connection.Query<CelConfigDto>("select * from cel_config where config_id = @ConfigId", queryParams);
            if (cels == null)
            {
                var config = connection.QuerySingle<ConfigurationDto>(
                    "select * from configuration where kisekae_id = @Id and id = @ConfigId",
                    queryParams);

                foreach (var line in config.Data.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
                    switch (line.ToCharArray()[0])
                    {
                        case '(':
                            var resolutionRegex = new Regex(ResolutionRegexPattern);
                            var resolutionMatch = resolutionRegex.Match(line);
                            config.Width = int.Parse(resolutionMatch.Groups["Width"].Value);
                            config.Height = int.Parse(resolutionMatch.Groups["Height"].Value);
                            break;
                        case '[':
                            var borderValue = line[1..];
                            if (borderValue.Contains(';')) borderValue = borderValue.Split(';')[0].Trim();
                            config.BorderIndex = int.Parse(borderValue);
                            break;
                        case '%':
                            break;
                        case '#':
                            break;
                        case '$':
                        case ' ':
                            break;
                    }

                await connection.UpdateAsync(config);
            }

            await connection.CloseAsync();
        }

        public async void StoreToDatabase(IFormFile file)
        {
            var memoryStream = await GetAsMemoryStream(file);
            var checksum = Convert.ToBase64String(await new SHA256Managed().ComputeHashAsync(memoryStream));
            memoryStream.Position = 0; // Reset for re-read

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var transaction = await connection.BeginTransactionAsync();

            try
            {
                var existing = GetExisting(connection, file.FileName, checksum);
                if (existing != null) return;

                _fileParserService.UnzipLzh(file, memoryStream);
                var directory = Path.GetFileNameWithoutExtension(file.FileName);
                var kisekae = new KisekaeDto
                {
                    Filename = file.FileName,
                    Name = directory,
                    Checksum = checksum
                };

                kisekae.Id = await connection.InsertAsync(kisekae);

                var filenames = _storage.GetFileNames($"{Path.Combine(directory, "*")}");
                await SetInnerFiles(connection, directory, filenames, kisekae.Id);

                using var saved = await connection.QueryMultipleAsync(
                    "select * from configuration where kisekae_id = @id; select * from cel where kisekae_id = @id; select * from palette where kisekae_id = @id",
                    new { id = kisekae.Id });
                var configs = saved.Read<ConfigurationDto>().ToList();
                var cels = saved.Read<CelDto>().ToDictionary(x => x.Filename.ToLowerInvariant());
                var palettes = saved.Read<PaletteDto>().ToDictionary(x => x.Filename.ToLowerInvariant());

                foreach (var config in configs)
                {
                    _configurationReaderService.ReadConfigurationToDto(config, cels, palettes);
                    await connection.InsertAsync<CelConfigDto>(config.Cels);
                    await connection.UpdateAsync<PaletteDto>(config.Palettes);
                    await connection.UpdateAsync(config);
                }

                var paletteColors = SetPaletteColors(palettes.Values);
                await connection.InsertAsync<PaletteColorDto>(paletteColors);

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                _logger.LogError(e, $"Error saving file {file.FileName}");
            }

            await connection.CloseAsync();
        }

        private KisekaeDto GetExisting(IDbConnection connection, string filename, string checksum)
        {
            var existing = connection.QuerySingleOrDefault<KisekaeDto>("select * from kisekae where kisekae.filename = @Filename", new { Filename = filename });
            if (existing != null)
            {
                _logger.LogInformation($"Already existing upload {filename} under id {existing.Id}");
                return existing;
            }

            existing = connection.QuerySingleOrDefault<KisekaeDto>("select * from kisekae where kisekae.checksum = @Checksum", new { Checksum = checksum });
            if (existing == null) return null;

            _logger.LogInformation($"Already existing upload (checksum: {checksum}) under id {existing.Id} and filename {existing.Filename}");
            return existing;
        }

        private async Task SetInnerFiles(IDbConnection connection, string directory, IEnumerable<string> filenames, int id)
        {
            var files = new List<IKisekaeFile>();
            foreach (var filename in filenames)
            {
                await using var reader = _storage.OpenFile(Path.Combine(directory, filename), FileMode.Open);
                if (!reader.CanRead) throw new InvalidDataException();

                var bytes = new byte[reader.Length];
                reader.Read(bytes, 0, bytes.Length);

                switch (Path.GetExtension(filename).ToLower())
                {
                    case ".cel":
                        files.Add(new CelDto
                        {
                            Filename = filename,
                            Data = bytes,
                            KisekaeId = id
                        });
                        break;
                    case ".cnf":
                        files.Add(new ConfigurationDto
                        {
                            Filename = filename,
                            Data = Encoding.ASCII.GetString(bytes),
                            KisekaeId = id
                        });
                        break;
                    case ".kcf":
                        files.Add(new PaletteDto
                        {
                            Filename = filename,
                            Data = bytes,
                            KisekaeId = id
                        });
                        break;
                }
            }

            await connection.InsertAsync(files.OfType<CelDto>());
            await connection.InsertAsync(files.OfType<ConfigurationDto>());
            await connection.InsertAsync(files.OfType<PaletteDto>());
        }

        private List<PaletteColorDto> SetPaletteColors(IEnumerable<PaletteDto> palettes)
        {
            var paletteColors = new List<PaletteColorDto>();
            foreach (var palette in palettes)
            {
                var colors = _fileParserService.ParsePalette(palette, out var groups, out var colorsPerGroup);

                for (var groupIndex = 0; groupIndex < groups; groupIndex++)
                for (var colorIndex = 0; colorIndex < colorsPerGroup; colorIndex++)
                {
                    var color = colors[groupIndex * colorsPerGroup + colorIndex];
                    paletteColors.Add(new PaletteColorDto
                    {
                        Group = groupIndex,
                        PaletteId = palette.Id,
                        Hex = color.ToHex()
                    });
                }
            }

            return paletteColors;
        }

        private static async Task<MemoryStream> GetAsMemoryStream(IFormFile file)
        {
            var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            return stream;
        }
    }
}