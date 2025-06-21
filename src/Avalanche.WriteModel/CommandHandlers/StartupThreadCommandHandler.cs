using Avalanche.WriteModel.Commands;
using Broadcast;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class StartupThreadCommandHandler : CommandHandler<StartupThreadCommand>
    {
        private readonly IEventBus _eventBus;

        public StartupThreadCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public override void Handle(StartupThreadCommand cmd)
        {
            var @event = new Events.RampupEvent
            {
                TestId = cmd.TestId,
                Name = cmd.Name,
                ElapsedMilliseconds = cmd.ElapsedMilliseconds,
                IsWarmup = cmd.IsWarmup
            };

            _eventBus.Publish(cmd.TestId, cmd.Time, @event);
        }
    }
}
