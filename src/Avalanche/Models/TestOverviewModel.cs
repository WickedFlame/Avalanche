using Avalanche.Runner;

namespace Avalanche.Models
{
    public class TestOverviewModel
    {
        public string Scenario { get; set; }

        public string TestId { get; set; }

        public DateTime? StartTime { get; set; }

        public TestSettings Settings { get; set; }

        public TestRunStatus Status { get; set; }
    }
}
