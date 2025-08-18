using Avalanche.Domain;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Admin")]
        public IActionResult RecreateDatabase()
        {
            _facade.RecreateDatabase();

            return RedirectToAction("Index");
        }

        //[Authorize(Roles = "Admin")]
        //public IActionResult UserManagement()
        //{
        //    return View();
        //}
    }
}
