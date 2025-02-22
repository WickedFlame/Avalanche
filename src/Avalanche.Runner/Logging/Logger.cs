namespace Avalanche.Runner.Logging
{
    //
    // Logger is per Testrun
    // LogCollection is per thread
    //

    public class Logger
    {
        private readonly List<TestResultsCollection> _collections = [];
        private readonly List<ITestResultCollector> _resultsCollectors = [];

        private bool _isRunning = true;

        public DateTime StartTime { get; set; }

        public ITestResultCollector StartNew(string name)
        {
            if (!_isRunning)
            {
                return null;
            }

            var collection = new TestResultsCollection(name);
            _collections.Add(collection);

            var collector = new TestResultCollector(collection);
            _resultsCollectors.Add(collector);

            return collector;
        }

        public IEnumerable<TestResultsCollection> GetCollections()
        {
            return _collections.ToList();
        }

        public void End()
        {
            _isRunning = false;

            foreach (var collector in _resultsCollectors)
            {
                collector.End();
            }
        }
    }

}
