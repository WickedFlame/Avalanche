using Avalanche.CommandModel.Commands;

namespace Avalanche.CommandModel.CommandHandlers
{
    public class EndTestCommandHandler : ICommandHandler
    {
        private readonly IEventStore _store;
        private readonly IEventDispatcher _eventDispatcher;

        public EndTestCommandHandler(IEventStore store, IEventDispatcher eventDispatcher)
        {
            _store = store;
            _eventDispatcher = eventDispatcher;
        }

        public void Handle(ICommand command)
        {
            var cmd = command as EndTestCommand;
            //TODO: create the readmodel
            var @event = new Events.EndTestEvent
            {
                TestId = cmd.TestId,
                EndTime = cmd.EndTime,
                Status = cmd.Status
            };

            _store.Add(cmd.TestId, typeof(Events.EndTestEvent).AssemblyQualifiedName, cmd.EndTime, @event);

            //TODO: remove this to the readmodel
            _eventDispatcher.Send(@event);
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
