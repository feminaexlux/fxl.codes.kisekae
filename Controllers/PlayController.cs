using fxl.codes.kisekae.Models;
using fxl.codes.kisekae.Services;
using Microsoft.AspNetCore.Mvc;

namespace fxl.codes.kisekae.Controllers
{
    public class PlayController : Controller
    {
        private readonly DatabaseService _databaseService;

        public PlayController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public IActionResult Index(int id)
        {
            var config = _databaseService.GetConfig(id);

            return View(new PlaysetModel(config));
        }
    }
}