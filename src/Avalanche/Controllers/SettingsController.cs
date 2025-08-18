using Avalanche.Domain;
using Avalanche.Domain.UserManagement;
using Avalanche.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ISettingsFacade _facade;
        private readonly IAccountFacade _accountFacade;

        public SettingsController(ISettingsFacade facade, IAccountFacade accountFacade)
        {
            _facade = facade;
            _accountFacade = accountFacade;
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

        [Authorize(Roles = "Admin")]
        public IActionResult Users()
        {
            var model = new UsersModel
            {
                Users = _accountFacade.GetUsers()
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddUser(string username, string password, string confirmpassword, string name, bool isadmin)
        {
            if(string.IsNullOrEmpty(password) || password != confirmpassword)
            {
                throw new InvalidOperationException("Passwords don't match");
            }

            _accountFacade.CreateUser(username, password, name, isadmin ? ["Admin"] : Enumerable.Empty<string>());
            return RedirectToAction("Users");
        }
    }
}
