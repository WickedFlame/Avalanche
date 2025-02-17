
using MeasureMap;

namespace Avalanche.Domain
{
    public class TestResultsCollection
    {
        private static readonly object _lock = new();
        private static TestResultsCollection _instance;
        public static TestResultsCollection Instance => (_instance ??= new TestResultsCollection());

        private readonly Dictionary<string, TestResult> _results = [];


        //
        // StartNew Testrun

        public TestResult StartNew(string key)
        {
            lock (_lock)
            {
                var result = new TestResult
                {
                    Name = key,
                    Status = TestRunStatus.New,
                    LogEntries = new Runner.Logging.Logger()
                };

                _results[key.ToLower()] = result;

                return result;
            }
        }





        public TestResult GetResults(string key)
        {
            lock (_lock)
            {
                if (!_results.ContainsKey(key.ToLower()))
                {
                    return new TestResult { Name = key };
                }

                return _results[key.ToLower()];
            }
        }
    }
}
