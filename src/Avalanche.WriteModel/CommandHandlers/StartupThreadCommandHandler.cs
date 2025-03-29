using Avalanche.WriteModel.Commands;
using Broadcast;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class StartupThreadCommandHandler : CommandHandler<StartupThreadCommand>
    {
        private readonly IEventStore _store;
        private readonly IMessageBus _messageBus;

        public StartupThreadCommandHandler(IEventStore store, IMessageBus messageBus)
        {
            _store = store;
            _messageBus = messageBus;
        }

        public override void Handle(StartupThreadCommand cmd)
        {
            //TODO: create the readmodel
            var @event = new Events.RampupEvent
            {
                TestId = cmd.TestId,
                //ThreadId = cmd.Thread,
                Category = cmd.Category,
                Module = cmd.Module,
                Name = cmd.Name,
                Message = cmd.Message,
                StatusCode = cmd.StatusCode,
                ElapsedMilliseconds = cmd.ElapsedMilliseconds,
                IsWarmup = cmd.IsWarmup
            };

            _store.Add(cmd.TestId, cmd.Time, @event);

            //TODO: remove this to the readmodel
            _messageBus.Send(@event);
        }
    }
}
