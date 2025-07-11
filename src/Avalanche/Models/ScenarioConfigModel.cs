
namespace Avalanche.Models
{
    public class ScenarioConfigModel
    {
        public IEnumerable<string> Scenarios { get; set; }

        public RawScenarioModel Scenario { get; set; }
    }

    public class RawScenarioModel
    {
        public string Name { get; set; }

        public string RawContent { get; set; }
    }
}
