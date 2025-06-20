using Avalanche.WriteModel.Commands;
using Broadcast;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class IterationFailedCommandHandler : CommandHandler<IterationFailedCommand>
    {
        private readonly IEventBus _eventBus;

        public IterationFailedCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public override void Handle(IterationFailedCommand cmd)
        {
            var @event = new Events.IterationErrorEvent
            {
                TestId = cmd.TestId,
                TestName = cmd.TestName,
                Thread = cmd.Thread,
                Time = cmd.Time,
                StatusCode = $"{(int)cmd.StatusCode}",
                Message = cmd.Message,
            };

            _eventBus.Publish(cmd.TestId, cmd.Time, @event);
        }
    }
}
