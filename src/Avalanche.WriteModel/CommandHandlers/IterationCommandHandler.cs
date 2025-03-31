using Avalanche.WriteModel.Commands;
using Broadcast;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class IterationCommandHandler : CommandHandler<IterationCommand>
    {
        private readonly IEventBus _eventBus;

        public IterationCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
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

            _eventBus.Publish(cmd.TestId, cmd.Time, @event);
        }
    }
}
