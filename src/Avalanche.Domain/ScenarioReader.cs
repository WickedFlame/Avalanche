using Avalanche.Runner;

namespace Avalanche.Domain
{
    public class ScenarioReader
    {
        public Scenario GetScenario(string path)
        {
            var reader = new YamlMap.YamlFileReader();
            var settings = reader.Read<Scenario>(path);

            return settings;
        }
    }
}
