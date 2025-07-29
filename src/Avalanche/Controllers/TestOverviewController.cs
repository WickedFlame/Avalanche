using Avalanche.DataSource;
using Avalanche.Domain;
using Avalanche.Models;
using Avalanche.ReadModel.QueryHandlers;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace Avalanche.Controllers
{
    public class TestOverviewController : Controller
    {
        private readonly IProjectionConnectionBuilder _builder;
        private readonly ILoggerFactory _factory;

        public TestOverviewController(IProjectionConnectionBuilder builder, ILoggerFactory factory)
        {
            _builder = builder;
            _factory = factory;
        }

        public IActionResult Index(string scenario, string testid)
        {
            var trh = new TestRunQueryHandler(_builder);
            var lastRun = trh.Get(new ReadModel.Queries.GetTestRun { TestId = testid });

            var path = PathMapper.GetScenarioFile(scenario);

            var tsr = new ScenarioReader(_factory);
            var settings = tsr.GetScenario(path);

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
