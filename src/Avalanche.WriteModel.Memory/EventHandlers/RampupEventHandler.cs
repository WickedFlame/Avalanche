using Avalanche.WriteModel.Events;
using Broadcast;
using System;

namespace Avalanche.WriteModel.Memory.EventHandlers
{
    public class RampupEventHandler :
        IEventHandler<RampupEvent>,
        IEventHandler<RampdownEvent>
    {
        public RampupEventHandler()
        {
        }

        public void Handle(RampupEvent @event)
        {
            if (@event.IsWarmup)
            {
                return;
            }

        }

        public void Handle(RampdownEvent @event)
        {
            if (@event.IsWarmup)
            {
                return;
            }

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
