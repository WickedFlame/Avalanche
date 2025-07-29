using Avalanche.WriteModel.Commands;
using Avalanche.WriteModel.Events;
using Broadcast;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class DeleteTestCommandHandler : CommandHandler<DeleteTestRunCommand>
    {
        private readonly IEventBus _eventBus;

        public DeleteTestCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public override void Handle(DeleteTestRunCommand cmd)
        {
            _eventBus.Publish(cmd.TestId, DateTime.Now, new DeleteTestRunEvent
            {
                TestId = cmd.TestId,
            });
        }
    }
}
