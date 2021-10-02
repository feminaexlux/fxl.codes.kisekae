using System;
using System.Diagnostics;
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

        public HomeController(ILogger<HomeController> logger, ConfigurationReaderService readerService)
        {
            _logger = logger;
            _readerService = readerService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoadConfiguration(IFormFile file)
        {
            if (!(file?.FileName.EndsWith("cnf", StringComparison.InvariantCultureIgnoreCase) ?? false))
                throw new Exception("Please select a *.cnf file");
            
            var model = _readerService.ReadCnf(file);
            return View("Index");
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