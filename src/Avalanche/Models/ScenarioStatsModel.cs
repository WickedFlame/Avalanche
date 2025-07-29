using Avalanche.ReadModel.Models;
using Avalanche.Runner;

namespace Avalanche.Models
{
    public class ScenarioStatsModel
    {
        public string Name { get; set; }

        public Scenario Settings { get; set; }

        public IEnumerable<TestStatistic> Stats { get; set; }

        public string Tab { get; set; }
    }
}
