using Avalanche.WriteModel.Commands;
using Broadcast;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class StartupThreadCommandHandler : ICommandHandler
    {
        private readonly IEventStore _store;
        private readonly IMessageBus _messageBus;

        public StartupThreadCommandHandler(IEventStore store, IMessageBus messageBus)
        {
            _store = store;
            _messageBus = messageBus;
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
