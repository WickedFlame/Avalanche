using Avalanche.Domain;
using Microsoft.Extensions.Logging;
using Moq;

namespace Avalanche.IntegrationTests.Domain
{
    public class ScenarioReaderTests
    {
        private ScenarioReader _reader;

        [SetUp]
        public void SetUp()
        {
            _reader = new ScenarioReader(Mock.Of<ILoggerFactory>());
        }

        [Test]
        public void ScenarioReader_GlobalConfig()
        {
            var scenario = _reader.GetScenario("scenarios/globalconfig.yml");
            scenario.TestCases.Should().AllBeEquivalentTo(new
            {
                Iterations = 10,
                Users = 11,
                Duration = 12,
                Interval = 13,
                RampupTime = 14,
                UseCookies = true,
                Delay = 15
            });
        }

        [Test]
        public void ScenarioReader_OverrideConfig()
        {
            var scenario = _reader.GetScenario("scenarios/overrideconfig.yml");
            scenario.TestCases.Should().AllBeEquivalentTo(new
            {
                Iterations = 20,
                Users = 21,
                Duration = 22,
                Interval = 23,
                RampupTime = 24,
                UseCookies = true,
                Delay = 25
            });
        }

        [Test]
        public void ScenarioReader_NoConfig()
        {
            var scenario = _reader.GetScenario("scenarios/noconfig.yml");
            scenario.TestCases.Should().AllBeEquivalentTo(new
            {
                Iterations = 20,
                Users = 21,
                Duration = 22,
                Interval = 23,
                RampupTime = 24,
                UseCookies = true,
                Delay = 25
            });
        }
    }
}
