using Avalanche.WriteModel.Events;
using Broadcast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avalanche.WriteModel.Memory.EventHandlers
{
    public class IterationEventHandler :
        IEventHandler<IterationLogEvent>,
        IEventHandler<IterationErrorEvent>
    {
        private readonly object _lock = new();

        private readonly TimedDispatcher _dispatcher;

        private readonly Dictionary<string, Dictionary<int, IterationLogEvent>> _events = [];
        private readonly List<IterationErrorEvent> _errors = [];

        public IterationEventHandler()
        {
            _dispatcher = new(3000, () => DispatcherTask());
            _dispatcher.StartDispatcher();
        }

        public void Handle(IterationLogEvent @event)
        {
            if (@event.IsWarmup)
            {
                return;
            }

            lock (_lock)
            {
                if (!_events.ContainsKey(@event.TestName))
                {
                    _events[@event.TestName] = [];
                }

                _events[@event.TestName][@event.Thread] = @event;
            }
        }

        public void Handle(IterationErrorEvent @event)
        {
            if (@event.IsWarmup)
            {
                return;
            }

            lock (_lock)
            {
                _errors.Add(@event);
            }
        }

        private bool DispatcherTask()
        {
            lock(_lock)
            {
                Console.WriteLine("");
                foreach (var test in _events)
                {
                    var errors = _errors.Where(e => e.TestName == test.Key);
                    Console.WriteLine($"{test.Key}   Users: {test.Value.Count}, Throughput: -, Iterations: {test.Value.Sum(t => t.Value.Iterations)}, Errors: {errors.Count()}");
                }
            }


            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // do stuf here;
                _dispatcher.Close();
            }
        }
    }
}
