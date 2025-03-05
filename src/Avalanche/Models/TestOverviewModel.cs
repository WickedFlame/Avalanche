using Avalanche.WriteModel.Events;
using Avalanche.Domain;
using Avalanche.QueryModel.Models;
using Avalanche.Runner;
using Avalanche.Runner.Logging;

namespace Avalanche.Models
{
    public class TestOverviewModel
    {
        public string Name { get; set; }

        public IEnumerable<IterationLogEvent> LogEntries { get; set; }

        public DateTime? StartTime { get; set; }

        public TestSettings Settings { get; set; }

        public TestRunStatus Status { get; set; }

        public IEnumerable<Runner.TestResult> Results { get; set; }

        public IEnumerable<StartupLogEvent> StartupEntries { get; set; }

        public IEnumerable<EndLogEvent> EndLogEntries { get; set; }

        public IEnumerable<TestRun> Runs { get; set; }
    }
}
