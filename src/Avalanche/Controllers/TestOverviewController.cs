using Avalanche.WriteModel;
using Avalanche.WriteModel.Events;
using Avalanche.Domain;
using Avalanche.Models;
using Avalanche.ReadModel.QueryHandlers;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Controllers
{
    public class TestOverviewController : Controller
    {
        private readonly IEventStore _store;

        public TestOverviewController(IEventStore store)
        {
            _store = store;;
        }

        public IActionResult Index(string scenario, string testid)
        {
            var trh = new TestRunQueryHandler();
            var lastRun = trh.Get(new ReadModel.Queries.GetTestRun { TestId = testid });

            var path = $"./testfiles/{scenario}.yml";

            var facace = new TestFacade(_store);
            var settings = facace.GetTestSettings(path);

            var model = new TestOverviewModel
            {
                Scenario = scenario,
                TestId = lastRun?.TestId,
                StartTime = lastRun?.StartTime,
                Settings = settings,
                Status = lastRun?.Status == null ? Runner.TestRunStatus.New : new Runner.TestRunStatus(lastRun.Status)
            };

            return View(model);
        }
    }
}
