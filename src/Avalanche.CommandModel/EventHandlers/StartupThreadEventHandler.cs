using Avalanche.CommandModel.Events;

namespace Avalanche.CommandModel.EventHandlers
{
    public class StartupThreadEventHandler : IEventHandler
    {
        private readonly TestResultsCollection _collection;

        public StartupThreadEventHandler(TestResultsCollection collection)
        {
            _collection = collection;
        }

        public void Handle(IEvent @event)
        {
            _collection.Add(@event as LogEvent);
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
