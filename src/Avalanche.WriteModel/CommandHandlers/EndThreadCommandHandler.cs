using Avalanche.WriteModel.Commands;
using Broadcast;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class EndThreadCommandHandler : CommandHandler<EndThreadCommand>
    {
        private readonly IEventBus _eventBus;

        public EndThreadCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public override void Handle(EndThreadCommand cmd)
        {
            var @event = new Events.RampdownEvent
            {
                TestId = cmd.TestId,
                Category = cmd.Category,
                Module = cmd.Module,
                TestCase = cmd.TestCase,
                Message = cmd.Message,
                Thread = cmd.Thread,
                IsWarmup = cmd.IsWarmup
            };

            _eventBus.Publish(cmd.TestId, cmd.Time, @event);
        }
    }
}
