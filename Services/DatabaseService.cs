using System.IO;
using System.IO.IsolatedStorage;
using Dapper;
using fxl.codes.kisekae.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace fxl.codes.kisekae.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;
        private readonly FileParserService _fileParserService;
        private readonly IsolatedStorageFile _storage;

        public DatabaseService(IConfiguration configuration, FileParserService fileParserService)
        {
            _fileParserService = fileParserService;
            _connectionString = configuration.GetConnectionString("kisekae");
            _storage = IsolatedStorageFile.GetUserStoreForApplication();
        }

        public async void StoreToDatabase(IFormFile file)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var existing = connection.QuerySingle<Kisekae>("select * from kisekae where kisekae.filename = @Filename", file);
            if (existing != null) return;

            _fileParserService.UnzipLzh(file);

            var directory = Path.GetFileNameWithoutExtension(file.FileName);

            await connection.CloseAsync();
        }
    }
}