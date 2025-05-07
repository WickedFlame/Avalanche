using Avalanche.WriteModel.Events;
using System;

namespace Avalanche.WriteModel.Memory.EventHandlers
{
    public class TestRunEventHandler :
        IEventHandler<StartTestEvent>,
        IEventHandler<EndTestEvent>
    {
        public TestRunEventHandler()
        {
        }

        public void Handle(StartTestEvent evnt)
        {
        }

        public void Handle(EndTestEvent evnt)
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
