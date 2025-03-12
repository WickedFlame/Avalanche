using Avalanche.WriteModel.Commands;
using Broadcast;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class EndTestCommandHandler : CommandHandler<EndTestCommand>
    {
        private readonly IEventStore _store;
        private readonly IMessageBus _messageBus;

        public EndTestCommandHandler(IEventStore store, IMessageBus messageBus)
        {
            _store = store;
            _messageBus = messageBus;
        }

        public override void Handle(EndTestCommand cmd)
        {
            var @event = new Events.EndTestEvent
            {
                TestId = cmd.TestId,
                EndTime = cmd.EndTime,
                Status = cmd.Status
            };

            _store.Add(cmd.TestId, cmd.EndTime, @event);

            _messageBus.Send(@event);
        }
    }
}
