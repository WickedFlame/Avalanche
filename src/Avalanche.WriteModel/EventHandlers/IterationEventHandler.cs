using Avalanche.WriteModel.Events;
using System.Data.Common;

namespace Avalanche.WriteModel.EventHandlers
{
    public class IterationEventHandler : IEventHandler<IterationLogEvent>
    {
        private readonly TestResultsCollection _collection;

        public IterationEventHandler(TestResultsCollection collection)
        {
            _collection = collection;
        }

        public void Handle(IterationLogEvent @event)
        {
            _collection.Add(@event);
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
