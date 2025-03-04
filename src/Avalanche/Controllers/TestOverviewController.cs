using Avalanche.CommandModel;
using Avalanche.CommandModel.Events;
using Avalanche.Domain;
using Avalanche.Models;
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

            var events = results.GetCollections()?
                .SelectMany(c => c.Events) ?? Enumerable.Empty<LogEvent>();

            var logEntries = events.OfType<IterationLogEvent>().OrderBy(c => c.Time);

            var model = new TestOverviewModel
            {
                Name = name,
                StartTime = results.StartTime,
                Settings = settings,
                Results = results.Results,
                LogEntries = logEntries,
                StartupEntries = events.OfType<StartupLogEvent>().OrderBy(c => c.Time),
                EndLogEntries = events.OfType<EndLogEvent>().OrderBy(c => c.Time),
                Status = results.Status
            };

            return View(model);
        }
    }
}
