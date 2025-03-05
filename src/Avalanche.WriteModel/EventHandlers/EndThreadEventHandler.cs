using Avalanche.WriteModel.Events;

namespace Avalanche.WriteModel.EventHandlers
{
    public class EndThreadEventHandler : IEventHandler<EndLogEvent>
    {
        private readonly TestResultsCollection _collection;

        public EndThreadEventHandler(TestResultsCollection collection)
        {
            _collection = collection;
        }

        public void Handle(EndLogEvent @event)
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
                // do stuf here
            }
        }
    }
}
