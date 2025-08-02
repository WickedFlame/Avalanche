using Avalanche.Domain;
using Avalanche.ReadModel.Models;
using Avalanche.Runner;

namespace Avalanche.Models
{
    public class TestOverviewModel
    {
        public string Scenario { get; set; }

        public string TestId { get; set; }

        public string Tab { get; set; }

        public DateTime? StartTime { get; set; }

        public Scenario Settings { get; set; }

        public TestRunStatus Status { get; set; }

        public TestRunnerType Runner { get; set; }

        public Dictionary<string, IEnumerable<IterationItem>> Details { get; set; } = [];
    }
}
