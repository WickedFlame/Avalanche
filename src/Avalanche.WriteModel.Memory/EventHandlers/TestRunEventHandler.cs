using Avalanche.WriteModel.Events;
using System;

namespace Avalanche.WriteModel.Memory.EventHandlers
{
    public class TestRunEventHandler :
        IEventHandler<StartTestEvent>
    {
        public TestRunEventHandler()
        {
        }

        public void Handle(StartTestEvent evnt)
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
