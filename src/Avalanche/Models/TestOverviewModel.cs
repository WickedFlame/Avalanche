using Avalanche.Runner;
using Avalanche.WriteModel.Events;

namespace Avalanche.Models
{
    public class TestOverviewModel
    {
        public string Name { get; set; }

        public string TestId { get; set; }

        public IEnumerable<IterationLogEvent> LogEntries { get; set; }

        public DateTime? StartTime { get; set; }

        public TestSettings Settings { get; set; }

        public TestRunStatus Status { get; set; }

        public IEnumerable<Runner.TestResult> Results { get; set; }

        public IEnumerable<StartupLogEvent> StartupEntries { get; set; }

        public IEnumerable<EndLogEvent> EndLogEntries { get; set; }
    }
}
