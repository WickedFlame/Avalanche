namespace Avalanche.WriteModel
{
    public class EventQueue
    {
        private readonly object _lock = new();

        private readonly Dictionary<string, IterationElementsContainer> _events = [];

        public IEnumerable<string> Keys => _events.Keys;

        public IterationElementsContainer this[string key] => _events[key];

        public void Add(string key, IterationElementsContainer iterationElementsContainer)
        {
            _events.Add(key, iterationElementsContainer);
        }

        public bool ContainsKey(string key)
        {
            return _events.ContainsKey(key);
        }

        public IterationElementsContainer Dequeue(string key)
        {
            lock (_lock)
            {
                if (!_events.ContainsKey(key))
                {
                    return null;
                }

                var items = _events[key];

                _events[key] = [];

                return items;
            }
        }

        public IterationElementsContainer Get(string key)
        {
            lock (_lock)
            {
                if (!_events.ContainsKey(key))
                {
                    _events[key] = [];
                }

                return _events[key];
            }
        }
    }
}
