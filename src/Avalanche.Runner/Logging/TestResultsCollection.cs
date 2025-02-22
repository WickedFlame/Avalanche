namespace Avalanche.Runner.Logging
{
    public class TestResultsCollection
    {
        private readonly List<LogEvent> _events = new List<LogEvent>();

        public TestResultsCollection(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public string ThreadId { get; set; }

        public void Add(LogEvent metric)
        {
            _events.Add(metric);
        }

        public IEnumerable<LogEvent> Events => _events;
    }
}
