
using Avalanche.Runner;
using MeasureMap;

namespace Avalanche.Domain
{
    public class TestResultsCollection
    {
        private static readonly object _lock = new();
        private static TestResultsCollection _instance;
        public static TestResultsCollection Instance => (_instance ??= new TestResultsCollection());

        private readonly Dictionary<string, TestRunData> _results = [];


        //
        // StartNew Testrun

        public TestRunData StartNew(string testId, string name)
        {
            lock (_lock)
            {
                var result = new TestRunData
                {
                    TestId = testId,
                    Name = name,
                    Status = TestRunStatus.New
                };

                _results[name.ToLower()] = result;

                return result;
            }
        }





        public TestRunData GetResults(string key)
        {
            lock (_lock)
            {
                if (!_results.ContainsKey(key.ToLower()))
                {
                    return new TestRunData { Name = key };
                }

                return _results[key.ToLower()];
            }
        }
    }
}
