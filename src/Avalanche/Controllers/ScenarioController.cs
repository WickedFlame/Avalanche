using Avalanche.Domain;
using Avalanche.Models;
using Avalanche.ReadModel.QueryHandlers;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Controllers
{
    public class ScenarioController : Controller
    {
        public ScenarioController()
        {
        }

        public IActionResult Index(string name)
        {
            var path = $"./testfiles/{name}.yml";

            var tsr = new TestSettingsReader();
            var settings = tsr.GetTestSettings(path);

            var trh = new TestRunQueryHandler();
            var runs = trh.Get(new ReadModel.Queries.GetTestsQuery { Scenario = name });

            var lastRun = trh.Get(new ReadModel.Queries.GetLastTestQuery { Scenario = name });

            var model = new ScenarioModel
            {
                Name = name,
                StartTime = lastRun?.StartTime,
                Status = lastRun?.Status == null ? Runner.TestRunStatus.New : new Runner.TestRunStatus(lastRun.Status),
                Settings = settings,
                Runs = runs
            };

            return View(model);
        }
    }
}
