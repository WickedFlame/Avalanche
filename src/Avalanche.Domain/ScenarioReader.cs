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

                if(settings.Config != null)
                {
                    foreach(var tc in settings.TestCases)
                    {
                        if (settings.Config.UseCookies)
                        {
                            tc.UseCookies = true;
                        }

                        SetConfigValue(settings.Config.Iterations, tc.Iterations, i => tc.Iterations = i);
                        SetConfigValue(settings.Config.Users, tc.Users, i => tc.Users = i);
                        SetConfigValue(settings.Config.Duration, tc.Duration, i => tc.Duration = i);
                        SetConfigValue(settings.Config.Interval, tc.Interval, i => tc.Interval = i);
                        SetConfigValue(settings.Config.RampupTime, tc.RampupTime, i => tc.RampupTime = i);
                        SetConfigValue(settings.Config.Delay, tc.Delay, i => tc.Delay = i);

                    }
                }

                return settings;
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Error while reading Scenario located at {Path}", path);

                return new Scenario();
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
