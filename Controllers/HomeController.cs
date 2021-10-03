using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using fxl.codes.kisekae.Models;
using fxl.codes.kisekae.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace fxl.codes.kisekae.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ConfigurationReaderService _readerService;
        private readonly IsolatedStorageFile _storage;

        public HomeController(ILogger<HomeController> logger, ConfigurationReaderService readerService)
        {
            _logger = logger;
            _readerService = readerService;

            _storage = IsolatedStorageFile.GetUserStoreForApplication();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var directories = _storage.GetDirectoryNames();
            var model = new List<DirectoryModel>();

            foreach (var directory in directories)
            {
                var files = _storage.GetFileNames(Path.Combine(directory, "*.cnf"));
                if (files.Length <= 0) continue;
                
                var doll = new DirectoryModel(directory);
                foreach (var file in files)
                {
                    doll.Configurations.Add(new ConfigurationModel(file));
                }
                
                model.Add(doll);
            }
            
            return View(model);
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            _logger.LogTrace($"File uploaded: {file?.FileName}");
            if (!(file?.FileName.EndsWith("cnf", StringComparison.InvariantCultureIgnoreCase) ?? false))
                throw new Exception("Please select a *.cnf file");
            
            var model = _readerService.ReadCnf(file);
            return View("Play", model);
        }
        
        [HttpPost]
        public IActionResult Select(string directory, string file)
        {
            var stream = _storage.OpenFile(Path.Combine(directory, file), FileMode.Open);
            var model = _readerService.ParseStream(stream, directory);
            return View("Play", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}