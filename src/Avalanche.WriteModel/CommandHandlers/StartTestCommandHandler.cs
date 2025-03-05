using Avalanche.WriteModel.Commands;
using Broadcast;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class StartTestCommandHandler : CommandHandler<StartTestCommand>
    {
        private readonly IEventStore _store;
        private readonly IMessageBus _messageBus;

        public StartTestCommandHandler(IEventStore store, IMessageBus messageBus)
        {
            _store = store;
            _messageBus = messageBus;
        }

        public override void Handle(StartTestCommand cmd)
        {
            var @event = new Events.StartTestEvent
            {
                TestId = cmd.TestId,
                Scenario = cmd.Scenario,
                StartTime = cmd.StartTime,
                Status = cmd.Status
            };

            _store.Add(cmd.TestId, typeof(Events.StartTestEvent).AssemblyQualifiedName, cmd.StartTime, @event);

            _messageBus.Send(@event);
        }
    }
}
