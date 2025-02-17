using Avalanche;
using Avalanche.Controllers;
using Avalanche.Domain;
using Avalanche.Runner;
using Avalanche.Runner.Logging;

namespace Avalanche.Models
{
    public class TestOverviewModel
    {
        public string Name { get; set; }

        public IEnumerable<IterationLogEvent> LogEntries { get; set; }

        //public IEnumerable<Avalanche.TestResult> Results { get; set; }

        public DateTime? StartTime { get; set; }

        public TestSettings Settings { get; set; }

        //public Dictionary<string, IEnumerable<ChartData>> ChartData { get; internal set; }

        public TestRunStatus Status { get; set; }

        public IEnumerable<Runner.TestResult> Results { get; set; }

        public IEnumerable<StartupLogEvent> StartupEntries { get; set; }

        public IEnumerable<EndLogEvent> EndLogEntries { get; set; }
    }
}
