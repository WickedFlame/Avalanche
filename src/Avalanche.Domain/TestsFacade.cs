using Avalanche.DataSource;
using Avalanche.ReadModel.Queries;
using Avalanche.ReadModel.QueryHandlers;
using Avalanche.Runner;

namespace Avalanche.Domain
{
    public class TestsFacade
    {
        private readonly IProjectionConnectionBuilder _builder;

        public TestsFacade(IProjectionConnectionBuilder builder)
        {
            _builder = builder;
        }

        public IEnumerable<TestDefinition> GetScenarios()
        {
            var path = Environment.GetEnvironmentVariable("SCENARIO_PATH");
            var scenarios = Directory.GetFiles(path ?? "./scenarios");

            return scenarios.Select(f => GetDefinition(f));
        }

        private TestDefinition GetDefinition(string name)
        {
            var definition = new TestDefinition
            {
                Name = Path.GetFileName(name).Replace(".yml", string.Empty),
                State = TestRunStatus.New
            };

            var handler = new TestRunQueryHandler(_builder);
            var test = handler.Get(new GetLastTestQuery { Scenario = definition.Name});
            if (test != null)
            {
                definition.State = new TestRunStatus(test?.Status ?? "New");
            }

            return definition;
        }
    }
}
