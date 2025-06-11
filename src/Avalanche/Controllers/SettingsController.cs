using Avalanche.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ISettingsFacade _facade;

        public SettingsController(ISettingsFacade facade)
        {
            _facade = facade;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RecreateDatabase()
        {
            _facade.RecreateDatabase();

            return RedirectToAction("Index");
        }
    }
}
