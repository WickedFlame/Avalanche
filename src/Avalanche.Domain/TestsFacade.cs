namespace Avalanche.Domain
{
    public class TestsFacade
    {
        public IEnumerable<TestDefinition> GetAvailiableTests()
        {
            var path = Environment.GetEnvironmentVariable("TESTFILE_PATH");
            var testfiles = Directory.GetFiles(path ?? "./testfiles");
            return testfiles.Select(f => new TestDefinition
            {
                Name = Path.GetFileName(f).Replace(".yml", string.Empty),
                State = "unknown"
            });
        }
    }
}
