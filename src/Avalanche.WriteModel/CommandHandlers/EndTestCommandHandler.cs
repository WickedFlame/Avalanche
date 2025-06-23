using Avalanche.WriteModel.Commands;
using Broadcast;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class EndTestCommandHandler : CommandHandler<EndTestCommand>
    {
        private readonly IEventBus _eventBus;

        public EndTestCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public override void Handle(EndTestCommand cmd)
        {
            var @event = new Events.EndTestEvent
            {
                TestId = cmd.TestId,
                EndTime = cmd.EndTime,
                Status = TestRunStatus.Done
            };

            _eventBus.Publish(cmd.TestId, cmd.EndTime, @event);
        }
    }
}
