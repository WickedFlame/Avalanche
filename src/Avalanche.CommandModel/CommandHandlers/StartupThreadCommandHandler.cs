using Avalanche.CommandModel.Commands;

namespace Avalanche.CommandModel.CommandHandlers
{
    public class StartupThreadCommandHandler : ICommandHandler
    {
        private readonly IEventStore _store;
        private readonly TestResultsCollection _collection;

        public StartupThreadCommandHandler(IEventStore store, TestResultsCollection collection)
        {
            _store = store;
            _collection = collection;
        }

        public void Handle(ICommand command)
        {
            var cmd = command as StartupThreadCommand;

            //TODO: create the readmodel
            var @event = new Events.StartupLogEvent
            {
                Category = cmd.Category,
                Module = cmd.Module,
                Name = cmd.Name,
                Message = cmd.Message,
                StatusCode = cmd.StatusCode,
                ElapsedMilliseconds = cmd.ElapsedMilliseconds,
                IsWarmup = cmd.IsWarmup
            };

            _store.Add(cmd.TestId, typeof(Events.StartupLogEvent).AssemblyQualifiedName, cmd.Time, @event);

            //TODO: remove this to the readmodel
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
