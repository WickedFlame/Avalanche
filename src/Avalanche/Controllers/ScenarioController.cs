using Avalanche.Domain;
using Avalanche.Models;
using Avalanche.ReadModel.QueryHandlers;
using Avalanche.WriteModel;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Controllers
{
    public class ScenarioController : Controller
    {
        private readonly IEventStore _store;

        public ScenarioController(IEventStore store)
        {
            _store = store;;
        }

        public IActionResult Index(string name)
        {
            var path = $"./testfiles/{name}.yml";

            var facace = new TestFacade(_store);
            var settings = facace.GetTestSettings(path);

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
