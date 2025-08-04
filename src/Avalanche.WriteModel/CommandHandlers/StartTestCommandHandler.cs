using Avalanche.WriteModel.Commands;
using Broadcast;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class StartTestCommandHandler : CommandHandler<StartTestCommand>
    {
        private readonly IEventBus _eventBus;

        public StartTestCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public override void Handle(StartTestCommand cmd)
        {
            var @event = new Events.StartTestEvent
            {
                TestId = cmd.TestId,
                Scenario = cmd.Scenario,
                StartTime = cmd.StartTime,
                Status = TestRunStatus.Running,
                Runner = cmd.Runner
            };

            _eventBus.Publish(cmd.TestId, cmd.StartTime, @event);
        }
    }
}
