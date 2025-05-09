using Avalanche.WriteModel.Events;
using System;

namespace Avalanche.WriteModel.Memory.EventHandlers
{
    public class IterationEventHandler : IEventHandler<IterationLogEvent>
    {
        public void Handle(IterationLogEvent @event)
        {
            Console.WriteLine($"{{ TestId: {@event.TestId}, TestCase: {@event.Name}, ThreadId: {@event.Thread}, AverageMilliseconds: {@event.AverageMilliseconds}, Throughput: {@event.Througput}, Iterations: {@event.Iterations} }}");
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
