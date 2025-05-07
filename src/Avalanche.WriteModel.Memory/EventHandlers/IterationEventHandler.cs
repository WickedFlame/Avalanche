using Avalanche.WriteModel.Events;
using Broadcast;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Avalanche.WriteModel.Memory.EventHandlers
{
    public class IterationEventHandler : IEventHandler<IterationLogEvent>
    {
        private readonly Dictionary<string, IterationElementsContainer> _events = new();

        public void Handle(IterationLogEvent @event)
        {
            var key = $"{@event.TestId}_{@event.Name}_{@event.Thread}";
            if (!_events.ContainsKey(key))
            {
                _events.Add(key, new IterationElementsContainer());
            }

            var lst = _events[key];
            lst.Add(@event);

            // only write to db if the last update was more than 2 seconds ago
            if (!lst.IsCheckValid())
            {
                return;
            }

            Console.WriteLine($"{{ TestId: {@event.TestId}, TestCase: {@event.Name}, ThreadId: {@event.Thread}, AverageMilliseconds: {lst.GetAverageMilliseconds()}, Throughput: {lst.GetThroughput()}, Iterations: {lst.Count()} }}");
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
            }
        }
    }
}
