using Avalanche.ReadModel.Queries;
using Avalanche.ReadModel.QueryHandlers;
using Avalanche.Runner;
using System.Xml.Linq;

namespace Avalanche.Domain
{
    public class TestsFacade
    {
        public IEnumerable<TestDefinition> GetAvailiableTests()
        {
            var path = Environment.GetEnvironmentVariable("TESTFILE_PATH");
            var testfiles = Directory.GetFiles(path ?? "./testfiles");

            return testfiles.Select(f => GetDefinition(f));
        }

        private TestDefinition GetDefinition(string name)
        {
            var definition = new TestDefinition
            {
                Name = Path.GetFileName(name).Replace(".yml", string.Empty),
                State = TestRunStatus.New
            };

            var handler = new TestRunQueryHandler();
            var test = handler.Get(new GetLastTestQuery { Scenario = definition.Name});
            if (test != null)
            {
                definition.State = new TestRunStatus(test?.Status ?? "New");
            }

            return definition;
        }
    }
}
