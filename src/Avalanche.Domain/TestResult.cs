using Avalanche;
using Avalanche.Runner;
using Avalanche.Runner.Logging;
using MeasureMap;

namespace Avalanche.Domain
{
    public class TestResult
    {
        public string Name { get; set; }

        public IEnumerable<Avalanche.Runner.TestResult> Results { get; set; }

        public Logger LogEntries { get; set; }

        public TestSettings Settings { get; set; }

        public TestRunStatus Status { get; set; }
    }
}
