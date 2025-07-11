using Avalanche.Runner;
using Microsoft.Extensions.Logging;

namespace Avalanche.Domain
{
    public class ScenarioReader
    {
        private readonly ILogger<ScenarioReader> _logger;

        public ScenarioReader(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger<ScenarioReader>();
        }

        public Scenario GetScenario(string path)
        {
            try
            {
                var reader = new YamlMap.YamlFileReader();
                var settings = reader.Read<Scenario>(path);

                return settings;
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Error while reading Scenario located at {Path}", path);

                return new Scenario();
            }
        }

        public void SaveScenario(string path , Scenario evnt)
        {
            var writer = new YamlMap.YamlFileWriter();
            writer.Write(path, evnt);
        }
    }
}
