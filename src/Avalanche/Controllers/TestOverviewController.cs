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

        public IActionResult Index(string name)
        {
            var trh = new TestRunQueryHandler();
            var lastRun = trh.Get(new ReadModel.Queries.GetLastTestQuery { Scenario = name });

            var path = $"./testfiles/{name}.yml";

            var facace = new TestFacade(_store);
            var settings = facace.GetTestSettings(path);



            var model = new TestOverviewModel
            {
                Name = name,
                TestId = lastRun?.TestId,
                StartTime = lastRun?.StartTime,
                Settings = settings,
                //Results = results.Results,
                //LogEntries = logEntries,
                //StartupEntries = events.OfType<StartupLogEvent>().OrderBy(c => c.Time),
                //EndLogEntries = events.OfType<EndLogEvent>().OrderBy(c => c.Time),
                LogEntries = Enumerable.Empty<IterationLogEvent>(),
                StartupEntries = Enumerable.Empty<StartupLogEvent>(),
                EndLogEntries = Enumerable.Empty<EndLogEvent>(),

                Status = lastRun?.Status == null ? Runner.TestRunStatus.New : new Runner.TestRunStatus(lastRun.Status)
            };

            return View(model);
        }
    }
}
