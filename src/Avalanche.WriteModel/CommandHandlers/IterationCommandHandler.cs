using Avalanche.WriteModel.Commands;
using Broadcast;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class IterationCommandHandler : CommandHandler<IterationCommand>
    {
        private readonly IEventBus _eventBus;
        private readonly EventQueue _events = new();
        private readonly EventQueue _cache = new();
        private readonly TimedDispatcher _dispatcher;

        public IterationCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _dispatcher = new(2000, () => DispatcherTask());
            _dispatcher.StartDispatcher();
        }

        public override void Handle(IterationCommand cmd)
        {
            //
            // the storage has to be per thread to ensure enough events are sent for the charts
            var key = $"{cmd.TestId}_{cmd.TestName}_{cmd.Thread}";
            if (!_events.ContainsKey(key))
            {
                _events.Add(key, new IterationElementsContainer());
            }

            _events[key].Add(cmd);

            _dispatcher.Continue();
        }

        public bool DispatcherTask()
        {
            foreach (var key in _events.Keys.ToList())
            {
                var entry = _events.Dequeue(key);
                if (entry != null && entry.Any())
                {
                    var cached = _cache.Get(key);
                    cached.Merge(entry);

                    if (cached.Count() == 0)
                    {
                        continue;
                    }

                    var cmd = entry.Last();

                    var @event = new Events.IterationLogEvent
                    {
                        TestId = cmd.TestId,
                        TestName = cmd.TestName,
                        Thread = cmd.Thread,
                        Time = cmd.Time,
                        IsWarmup = cmd.IsWarmup,
                        //
                        // GetThroughput should be on all elementst/threads instead of only the current thread
                        Throughput = cached.GetThroughput(),
                        Iterations = cached.Count(),
                        AverageMilliseconds = cached.GetAverageMilliseconds()
                    };

                    _eventBus.Publish(cmd.TestId, cmd.Time, @event);

                    Console.WriteLine($"Sent events for {key}");

                    if (!_dispatcher.IsRunning)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            _dispatcher.Close();

            base.Dispose(disposing);
        }
    }

    public class EventQueue
    {
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
            if (!_events.ContainsKey(key))
            {
                return null;
            }

            var items = _events[key];

            _events[key] = new IterationElementsContainer();

            return items;
        }

        public IterationElementsContainer Get(string key)
        {
            if (!_events.ContainsKey(key))
            {
                _events[key] = new IterationElementsContainer();
            }

            return _events[key];
        }
    }
}
