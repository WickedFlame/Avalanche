using Avalanche.ReadModel.Models;
using Avalanche.Runner;

namespace Avalanche.Models
{
    public class ScenarioModel
    {
        public string Name { get; set; }

        public TestSettings Settings { get; set; }

        public DateTime? StartTime { get; set; }

        public TestRunStatus Status { get; set; }

        public IEnumerable<TestRun> Runs { get; set; }        
    }
}
