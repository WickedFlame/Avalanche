using Avalanche.WriteModel.Commands;
using Broadcast;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class EndThreadCommandHandler : CommandHandler<EndThreadCommand>
    {
        private readonly IEventStore _store;
        private readonly IMessageBus _messageBus;

        public EndThreadCommandHandler(IEventStore store, IMessageBus messageBus)
        {
            _store = store;
            _messageBus = messageBus;
        }

        public override void Handle(EndThreadCommand cmd)
        {
            //TODO: create the readmodel
            var @event = new Events.EndLogEvent
            {
                TestId = cmd.TestId,
                Category = cmd.Category,
                Module = cmd.Module,
                Name = cmd.Name,
                Message = cmd.Message,
                Thread = cmd.Thread,
                IsWarmup = cmd.IsWarmup
            };

            _store.Add(cmd.TestId, typeof(Events.EndLogEvent).AssemblyQualifiedName, cmd.Time, @event);


            //TODO: remove this to the readmodel
            _messageBus.Send(@event);
        }
    }
}
