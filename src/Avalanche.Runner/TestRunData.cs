using Avalanche;
using Avalanche.WriteModel;
using Avalanche.Runner;
using Avalanche.Runner.Logging;
using MeasureMap;

namespace Avalanche.Runner
{
    public class TestRunData
    {
        public string TestId { get; set; }

        public string Name { get; set; }

        public IEnumerable<Avalanche.Runner.TestResult> Results { get; set; }

        public List<TestResultsCollection> Collections { get; } = [];

        public TestSettings Settings { get; set; }

        public TestRunStatus Status { get; set; }

        public DateTime StartTime { get; set; }
    }
}
