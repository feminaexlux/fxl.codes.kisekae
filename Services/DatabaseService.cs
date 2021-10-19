using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Dapper;
using fxl.codes.kisekae.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace fxl.codes.kisekae.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;
        private readonly FileParserService _fileParserService;
        private readonly ILogger<DatabaseService> _logger;
        private readonly IsolatedStorageFile _storage;

        public DatabaseService(ILogger<DatabaseService> logger, IConfiguration configuration, FileParserService fileParserService)
        {
            _logger = logger;
            _fileParserService = fileParserService;
            _connectionString = configuration.GetConnectionString("kisekae");
            _storage = IsolatedStorageFile.GetUserStoreForApplication();
        }

        public async Task<KisekaeDto> StoreToDatabase(IFormFile file)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var existing = connection.QuerySingleOrDefault<KisekaeDto>("select * from kisekae where kisekae.filename = @Filename", new { Filename = file.FileName });
            if (existing != null)
            {
                _logger.LogInformation($"Already existing upload {file.FileName} under id {existing.Id}");
                return existing;
            }

            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var checksum = Convert.ToBase64String(await new SHA256Managed().ComputeHashAsync(memoryStream));
            existing = connection.QuerySingleOrDefault<KisekaeDto>("select * from kisekae where kisekae.checksum = @Checksum", new { Checksum = checksum });
            if (existing != null)
            {
                _logger.LogInformation($"Already existing upload (checksum: {checksum}) under id {existing.Id} and filename {existing.Filename}");
                return existing;
            }

            memoryStream.Position = 0;

            _fileParserService.UnzipLzh(file, memoryStream);
            var directory = Path.GetFileNameWithoutExtension(file.FileName);

            var kisekae = new KisekaeDto
            {
                Filename = file.FileName,
                Name = directory,
                Checksum = checksum
            };

            var id = connection.QueryFirst<int>("insert into kisekae(name, filename, checksum) values (@Name, @Filename, @Checksum) returning id", kisekae);
            kisekae.Id = id;

            var filenames = _storage.GetFileNames($"{Path.Combine(directory, "*")}");
            var cels = new List<CelDto>();
            var palettes = new List<PaletteDto>();
            var configurations = new List<ConfigurationDto>();
            foreach (var filename in filenames)
            {
                await using var reader = _storage.OpenFile(Path.Combine(directory, filename), FileMode.Open);
                if (!reader.CanRead) throw new InvalidDataException();

                var bytes = new byte[reader.Length];
                reader.Read(bytes, 0, bytes.Length);

                switch (Path.GetExtension(filename).ToLower())
                {
                    case ".cel":
                        cels.Add(new CelDto
                        {
                            Filename = filename,
                            Data = bytes,
                            KisekaeId = id
                        });
                        break;
                    case ".cnf":
                        configurations.Add(new ConfigurationDto
                        {
                            Filename = filename,
                            Data = BitConverter.ToString(bytes),
                            KisekaeId = id
                        });
                        break;
                    case ".kcf":
                        palettes.Add(new PaletteDto
                        {
                            Filename = filename,
                            Data = bytes,
                            KisekaeId = id
                        });
                        break;
                }
            }

            await connection.ExecuteAsync("insert into cel (filename, data, kisekae_id) values (@Filename, @Data, @KisekaeId)", cels);
            await connection.ExecuteAsync("insert into configuration (filename, data, kisekae_id) values (@Filename, @Data, @KisekaeId)", configurations);
            await connection.ExecuteAsync("insert into palette (filename, data, kisekae_id) values (@Filename, @Data, @KisekaeId)", palettes);

            await connection.CloseAsync();

            return kisekae;
        }
    }
}