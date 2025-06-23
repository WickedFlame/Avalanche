using Avalanche.DataSource;
using Avalanche.Domain;
using Avalanche.Models;
using Avalanche.ReadModel.QueryHandlers;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Controllers
{
    public class TestOverviewController : Controller
    {
        private readonly IProjectionConnectionBuilder _builder;

        public TestOverviewController(IProjectionConnectionBuilder builder)
        {
            _builder = builder;
        }

        public IActionResult Index(string scenario, string testid)
        {
            var trh = new TestRunQueryHandler(_builder);
            var lastRun = trh.Get(new ReadModel.Queries.GetTestRun { TestId = testid });

            var path = $"./testfiles/{scenario}.yml";

            var tsr = new TestSettingsReader();
            var settings = tsr.GetTestSettings(path);

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
