using Avalanche.WriteModel.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalanche.WriteModel.Memory.EventHandlers
{
    public class SummaryEventHandler :
        IEventHandler<ThreadSummaryEvent>,
        IEventHandler<TestSummaryEvent>
    {
        public SummaryEventHandler()
        {
        }

        public void Handle(ThreadSummaryEvent @event)
        {
        }

        public void Handle(TestSummaryEvent @event)
        {
            Console.WriteLine($"{@event.TestCase}, Average: {@event.AverageMilliseconds}ms");
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
                // do stuf here
            }
        }
    }
}
