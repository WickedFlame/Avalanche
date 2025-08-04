
namespace Avalanche.Models
{
    public class ScenarioEditorModel : RawScenarioModel
    {
        public IEnumerable<string> Scenarios { get; set; }
    }

    public class RawScenarioModel
    {
        public string Name { get; set; }

        public string RawContent { get; set; }
    }
}
