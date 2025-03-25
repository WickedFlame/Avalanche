using Avalanche.WriteModel.Commands;
using Broadcast;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class IterationCommandHandler : CommandHandler<IterationCommand>
    {
        private readonly IEventStore _store;
        private readonly IMessageBus _messageBus;

        public IterationCommandHandler(IEventStore store, IMessageBus messageBus)
        {
            _store = store;
            _messageBus = messageBus;
        }

        public override void Handle(IterationCommand cmd)
        {
            var @event = new Events.IterationLogEvent
            {
                TestId = cmd.TestId,
                Category = cmd.Category,
                Module = cmd.Module,
                Name = cmd.Name,
                Message = cmd.Message,
                RunNumber = cmd.RunNumber,
                Thread = cmd.Thread,
                IsWarmup = cmd.IsWarmup,
                TotalMilliseconds = cmd.TotalMilliseconds,
                Time = cmd.Time
            };

            _store.Add(cmd.TestId, cmd.Time, @event);

            _messageBus.Send(@event);
        }
    }
}
