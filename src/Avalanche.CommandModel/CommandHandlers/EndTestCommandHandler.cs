using Avalanche.CommandModel.Commands;
using Broadcast;

namespace Avalanche.CommandModel.CommandHandlers
{
    public class EndTestCommandHandler : ICommandHandler
    {
        private readonly IEventStore _store;
        private readonly IMessageBus _messageBus;

        public EndTestCommandHandler(IEventStore store, IMessageBus messageBus)
        {
            _store = store;
            _messageBus = messageBus;
        }

        public void Handle(ICommand command)
        {
            var cmd = command as EndTestCommand;
            
            var @event = new Events.EndTestEvent
            {
                TestId = cmd.TestId,
                EndTime = cmd.EndTime,
                Status = cmd.Status
            };

            _store.Add(cmd.TestId, typeof(Events.EndTestEvent).AssemblyQualifiedName, cmd.EndTime, @event);

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
