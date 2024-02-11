using System;
using System.Diagnostics;
using System.Linq;
using fxl.codes.kisekae.Models;
using fxl.codes.kisekae.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace fxl.codes.kisekae.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DatabaseService databaseService)
        {
            _logger = logger;
            _databaseService = databaseService;
        }

        public IActionResult Index()
        {
            var files = _databaseService.GetAll();
            return View(files.Select(x => new KisekaeModel(x)));
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            _logger.LogTrace($"File uploaded: {file?.FileName}");
            if (!(file?.FileName.EndsWith("lzh", StringComparison.InvariantCultureIgnoreCase) ?? false))
                throw new Exception("Please select a *.lzh file");

            _databaseService.StoreToDatabase(file);

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}