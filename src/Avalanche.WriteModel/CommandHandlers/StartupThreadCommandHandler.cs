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
                //ThreadId = cmd.Thread,
                Category = cmd.Category,
                Module = cmd.Module,
                Name = cmd.Name,
                Message = cmd.Message,
                StatusCode = cmd.StatusCode.ToString(),
                ElapsedMilliseconds = cmd.ElapsedMilliseconds,
                IsWarmup = cmd.IsWarmup
            };

            _eventBus.Publish(cmd.TestId, cmd.Time, @event);
        }
    }
}
