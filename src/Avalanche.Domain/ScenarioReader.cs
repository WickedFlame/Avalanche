using Avalanche.Runner;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Avalanche.Domain
{
    public class ScenarioReader
    {
        private readonly ILogger<ScenarioReader> _logger;

        public ScenarioReader(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger<ScenarioReader>();
        }

        public Scenario GetScenarioFromFile(string path)
        {
            try
            {
                var reader = new YamlMap.YamlFileReader();
                var scenario = reader.Read<Scenario>(path);

                MapConfigToTestCases(scenario);

                return scenario;
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Error while reading Scenario located at {Path}", path);

                return new Scenario();
            }
        }

        public Scenario GetScenarioFromYml(string yml)
        {
            try
            {
                var reader = new YamlMap.YamlReader();
                var scenario = reader.Read<Scenario>(yml);

                MapConfigToTestCases(scenario);

                return scenario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while reading Scenario from {Yaml}", yml);

                return new Scenario();
            }
        }

        private static void MapConfigToTestCases(Scenario scenario)
        {
            if (scenario.Config != null)
            {
                foreach (var tc in scenario.TestCases)
                {
                    if (scenario.Config.UseCookies)
                    {
                        tc.UseCookies = true;
                    }

                    SetConfigValue(scenario.Config.Iterations, tc.Iterations, i => tc.Iterations = i);
                    SetConfigValue(scenario.Config.Users, tc.Users, i => tc.Users = i);
                    SetConfigValue(scenario.Config.Duration, tc.Duration, i => tc.Duration = i);
                    SetConfigValue(scenario.Config.Interval, tc.Interval, i => tc.Interval = i);
                    SetConfigValue(scenario.Config.RampupTime, tc.RampupTime, i => tc.RampupTime = i);
                    SetConfigValue(scenario.Config.Delay, tc.Delay, i => tc.Delay = i);

                }
            }
        }

        private static void SetConfigValue(int config, int tc, Action<int> action)
        {
            if(config > 0 && tc == 0)
            {
                action(config);
            }
        }

        public void SaveScenario(string path , Scenario evnt)
        {
            var writer = new YamlMap.YamlFileWriter();
            writer.Write(path, evnt);
        }
    }
}
