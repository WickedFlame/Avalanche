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

        public void SaveScenario(string path , Scenario evnt)
        {
            var writer = new YamlMap.YamlFileWriter();
            writer.Write(path, evnt);
        }
    }
}
