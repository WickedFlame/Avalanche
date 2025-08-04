using Avalanche.DataSource;
using Avalanche.Domain;
using Avalanche.Models;
using Avalanche.ReadModel.Queries;
using Avalanche.ReadModel.QueryHandlers;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Index(string scenario, string testid, string tab)
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
                Tab = tab,
                StartTime = lastRun?.StartTime,
                Settings = settings,
                Status = lastRun?.Status == null ? Runner.TestRunStatus.New : new Runner.TestRunStatus(lastRun.Status),
                Runner = lastRun?.Runner == null ? new TestRunnerType("") : new TestRunnerType(lastRun.Runner)
            };

            if(tab == "details")
            {
                model.Details = trh.Get(new GetDetailData { TestId = testid });
            }

            return View(model);
        }
    }
}
