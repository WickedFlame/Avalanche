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

            var result = Domain.TestResultsCollection.Instance.GetResults(definition.Name.ToLower());

            if (result != null)
            {
                definition.State = result.Status ?? TestRunStatus.New;
            }

            return definition;
        }
    }
}
