using Microsoft.AspNetCore.Mvc;
using Avalanche.Domain;
using Avalanche.Models;
using Avalanche.Domain.Models;
using Avalanche.Runner.Logging;

namespace Avalanche.Controllers
{
    public class TestOverviewController : Controller
    {
        public IActionResult Index(string name)
        {
            var results = TestResultsCollection.Instance.GetResults(name.ToLower());
            var settings = results?.Settings;

            if (settings == null)
            {
                var path = $"./testfiles/{name}.yml";

                var facace = new TestFacade();
                settings = facace.GetTestSettings(path);
            }

            var events = results.LogEntries?.GetCollections()?
                .SelectMany(c => c.Events) ?? Enumerable.Empty<LogEvent>();

            var logEntries = events.OfType<IterationLogEvent>().OrderBy(c=>c.Time);

            var model = new TestOverviewModel
            {
                Name = name,
                StartTime = results.LogEntries?.StartTime,
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
