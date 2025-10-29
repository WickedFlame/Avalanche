using Avalanche.DataSource;
using Avalanche.Domain;
using Avalanche.Models;
using Avalanche.ReadModel.QueryHandlers;
using Avalanche.WriteModel;
using Broadcast;
using Microsoft.AspNetCore.Mvc;

namespace Avalanche.Controllers
{
    public class ScenarioController : Controller
    {
        private readonly IEventBus _eventBus;
        private readonly IProjectionConnectionBuilder _builder;
        private readonly ILoggerFactory _loggerFactory;

        public ScenarioController(IEventBus eventBus, IProjectionConnectionBuilder builder, ILoggerFactory loggerFactory)
        {
            _eventBus = eventBus;
            _builder = builder;
            _loggerFactory = loggerFactory;
        }

        public IActionResult Index(string name)
        {
            var path = PathMapper.GetScenarioFile(name);

            var tsr = new ScenarioReader(_loggerFactory);
            var settings = tsr.GetScenarioFromFile(path);

            var trh = new TestRunQueryHandler(_builder);
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

        public IActionResult Statistics(string name, string tab)
        {
            var path = PathMapper.GetScenarioFile(name);

            var tsr = new ScenarioReader(_loggerFactory);
            var settings = tsr.GetScenarioFromFile(path);

            var trh = new TestRunQueryHandler(_builder);
            var stats = trh.Get(new ReadModel.Queries.GetTestsStatisticsQuery {  Scenario = name });

            var model = new ScenarioStatsModel
            {
                Name = name,
                Settings = settings,
                Stats = stats,
                Tab = tab
            };

            return View(model);
        }

        public IActionResult DeleteScenario(string scenario)
        {
            var path = PathMapper.GetScenarioFile(scenario);

            var trh = new TestRunQueryHandler(_builder);
            var runs = trh.Get(new ReadModel.Queries.GetTestsQuery { Scenario = scenario });

            var dispatcher = new CommandDispatcher(_eventBus);
            var facade = new TestFacade(dispatcher, _loggerFactory);
            foreach (var run in runs)
            {
                facade.Delete(run.TestId);
            }

            System.IO.File.Delete(path);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult DeleteTestRun(string scenario, string testId)
        {
            var dispatcher = new CommandDispatcher(_eventBus);
            var facade = new TestFacade(dispatcher, _loggerFactory);
            facade.Delete(testId);

            return RedirectToAction(nameof(Index), new { name = scenario });
        }
    }
}
