using Avalanche.Runner;

namespace Avalanche.Domain
{
    public class TestSettingsReader
    {
        public TestSettings GetTestSettings(string path)
        {
            var reader = new YamlMap.YamlFileReader();
            var settings = reader.Read<TestSettings>(path);

            return settings;
        }
    }
}
