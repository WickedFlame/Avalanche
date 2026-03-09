using Avalanche.WriteModel.Events;
using Broadcast;
using System;

namespace Avalanche.WriteModel.Memory.EventHandlers
{
    public class SummaryEventHandler :
        IEventHandler<ThreadSummaryEvent>
    {
        public SummaryEventHandler()
        {
        }

        public void Handle(ThreadSummaryEvent @event)
        {
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
