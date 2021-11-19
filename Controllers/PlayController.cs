using System.Threading.Tasks;
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

        public async Task<IActionResult> Index(int id)
        {
            var model = await _databaseService.LoadConfig(id);
            
            return View(model);
        }
    }
}