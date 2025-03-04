using Avalanche.CommandModel.Commands;

namespace Avalanche.CommandModel.CommandHandlers
{
    public class StartTestCommandHandler : ICommandHandler
    {
        private readonly IEventStore _store;
        private readonly IEventDispatcher _eventDispatcher;

        public StartTestCommandHandler(IEventStore store, IEventDispatcher eventDispatcher)
        {
            _store = store;
            _eventDispatcher = eventDispatcher;
        }

        public void Handle(ICommand command)
        {
            var cmd = command as StartTestCommand;

            //TODO: create the readmodel
            var @event = new Events.StartTestEvent
            {
                TestId = cmd.TestId,
                TestName = cmd.TestName,
                StartTime = cmd.StartTime,
                Status = cmd.Status
            };

            _store.Add(cmd.TestId, typeof(Events.StartTestEvent).AssemblyQualifiedName, cmd.StartTime, @event);

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
