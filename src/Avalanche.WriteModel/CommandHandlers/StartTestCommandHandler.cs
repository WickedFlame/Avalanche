using Avalanche.WriteModel.Commands;
using Broadcast;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class StartTestCommandHandler : ICommandHandler
    {
        private readonly IEventStore _store;
        private readonly IMessageBus _messageBus;

        public StartTestCommandHandler(IEventStore store, IMessageBus messageBus)
        {
            _store = store;
            _messageBus = messageBus;
        }

        public void Handle(ICommand command)
        {
            var cmd = command as StartTestCommand;

            var @event = new Events.StartTestEvent
            {
                TestId = cmd.TestId,
                TestName = cmd.TestName,
                StartTime = cmd.StartTime,
                Status = cmd.Status
            };

            _store.Add(cmd.TestId, typeof(Events.StartTestEvent).AssemblyQualifiedName, cmd.StartTime, @event);

            _messageBus.Send(@event);
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
