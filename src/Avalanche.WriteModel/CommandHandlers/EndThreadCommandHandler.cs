using Avalanche.WriteModel.Commands;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class EndThreadCommandHandler : ICommandHandler
    {
        private readonly IEventStore _store;
        private readonly TestResultsCollection _collection;

        public EndThreadCommandHandler(IEventStore store, TestResultsCollection collection)
        {
            _store = store;
            _collection = collection;
        }

        public void Handle(ICommand command)
        {
            var cmd = command as EndThreadCommand;

            //TODO: create the readmodel
            var @event = new Events.EndLogEvent
            {
                Category = cmd.Category,
                Module = cmd.Module,
                Name = cmd.Name,
                Message = cmd.Message,
                Thread = cmd.Thread,
                IsWarmup = cmd.IsWarmup
            };

            _store.Add(cmd.TestId, typeof(Events.EndLogEvent).AssemblyQualifiedName, cmd.Time, @event);


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
