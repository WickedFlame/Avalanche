using Microsoft.AspNetCore.Mvc;
using Avalanche.Domain;
using Avalanche.Models;
using System.Diagnostics;
using Avalanche.DataSource;

namespace Avalanche.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProjectionConnectionBuilder _builder;

        public HomeController(ILogger<HomeController> logger, IProjectionConnectionBuilder builder)
        {
            _logger = logger;
            _builder = builder;
        }

        public IActionResult Index()
        {
            var facade = new TestsFacade(_builder);

            var model = new TestsViewModel
            {
                Tests = facade.GetAvailiableTests()
            };

            return View(model);
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
