
namespace Avalanche.Runner
{
    public class TestRunData
    {
        public string TestId { get; set; }

        public string Name { get; set; }

        public IEnumerable<Avalanche.Runner.TestResult> Results { get; set; }

        public TestSettings Settings { get; set; }

        public TestRunStatus Status { get; set; }

        public DateTime StartTime { get; set; }
    }
}
