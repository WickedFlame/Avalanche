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
            var results = Domain.TestResultsCollection.Instance.GetResults(name.ToLower());
            var settings = results?.Settings;

            if (settings == null)
            {
                var path = $"./testfiles/{name}.yml";

                var facace = new TestFacade(_store);
                settings = facace.GetTestSettings(path);
            }


            var trh = new TestRunQueryHandler();
            var runs = trh.Get(new ReadModel.Queries.GetTestsQuery { Scenario = name });

            var lastRun = trh.Get(new ReadModel.Queries.GetLastTestQuery { Scenario = name });


            var events = results.GetCollections()?
                .SelectMany(c => c.Events) ?? Enumerable.Empty<LogEvent>();

            var logEntries = events.OfType<IterationLogEvent>().OrderBy(c => c.Time);

            var model = new TestOverviewModel
            {
                Name = name,
                StartTime = lastRun.StartTime,
                Settings = settings,
                Runs = runs,
                Results = results.Results,
                LogEntries = logEntries,
                StartupEntries = events.OfType<StartupLogEvent>().OrderBy(c => c.Time),
                EndLogEntries = events.OfType<EndLogEvent>().OrderBy(c => c.Time),
                Status = new Runner.TestRunStatus(lastRun.Status)
            };

            return View(model);
        }
    }
}
