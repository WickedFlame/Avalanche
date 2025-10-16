using Avalanche.Domain.Settings;
using Avalanche.Domain.UserManagement;
using Avalanche.Models;
using Avalanche.ReadModel.Models;
using Avalanche.WriteModel.Events;
using Broadcast;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Avalanche.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ISettingsFacade _facade;
        private readonly IAccountFacade _accountFacade;
        private readonly IEventBus _eventBus;

        public SettingsController(ISettingsFacade facade, IAccountFacade accountFacade, IEventBus eventBus)
        {
            _facade = facade;
            _accountFacade = accountFacade;
            _eventBus = eventBus;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Profile()
        {
            var user = _accountFacade.GetUser(User.Identity.Name);

            var model = new ProfileModel
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Roles = user.Roles
            };

            if (!string.IsNullOrEmpty(TempData["error"]?.ToString()))
            {
                ViewBag.Error = TempData["error"]?.ToString();
                TempData["error"] = null;
            }

            return View(model);
        }

        public IActionResult ChangePassword(string userId, string oldpwd, string newpwd, string confirmpwd)
        {
            if(!_accountFacade.ChangePassword(userId, oldpwd, newpwd, confirmpwd))
            {
                TempData["error"] = "Invalid credentials";
            }

            return RedirectToAction("Profile");
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Database()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult RecreateDatabase()
        {
            _facade.RecreateDatabase();

            return RedirectToAction("Database");
        }




        [Authorize(Roles = "Admin")]
        public IActionResult ApiKeys()
        {
            var model = new ApiKeyModel
            {
                ApiKeys = _facade.GetApiKeys()
            };

            var newKey = TempData["apikey"]?.ToString();
            if (!string.IsNullOrEmpty(newKey))
            {
                model.NewKey = _facade.GetApiKey(newKey);
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddApiKey(string name, string expiration)
        {
            var expires = expiration switch
            {
                "1 Month" => Expiration.OneMonth,
                "3 Months" => Expiration.ThreeMonths,
                "6 Months" => Expiration.SixMonths,
                "12 Months" => Expiration.TwelveMonths,
                _ => Expiration.Never
            };

            _facade.AddApiKey(name, expires);

            TempData["apikey"] = name;

            return RedirectToAction("ApiKeys");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteApiKey(string name)
        {
            _facade.DeleteApiKey(name);
            return RedirectToAction("ApiKeys");
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

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(string userId)
        {
            _eventBus.Publish(new DeleteUserEvent { UserId = userId });
            return RedirectToAction("Users");
        }
    }
}
