using Avalanche.WriteModel.Events;
using System;

namespace Avalanche.WriteModel.Memory.EventHandlers
{
    public class IterationEventHandler :
        IEventHandler<IterationLogEvent>,
        IEventHandler<IterationErrorEvent>
    {
        public void Handle(IterationLogEvent @event)
        {
            Console.WriteLine($"{{ TestId: {@event.TestId}, TestCase: {@event.TestName}, ThreadId: {@event.Thread}, AverageMilliseconds: {@event.AverageMilliseconds}, Throughput: {@event.Throughput}, Iterations: {@event.Iterations} }}");
        }

        public void Handle(IterationErrorEvent @event)
        {
            Console.WriteLine($"{{ TestId: {@event.TestId}, TestCase: {@event.TestName}, ThreadId: {@event.Thread}, StatusCode: {@event.StatusCode}, Message: {@event.Message} }}");
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
