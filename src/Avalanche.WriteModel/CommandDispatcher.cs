using Avalanche.WriteModel.CommandHandlers;
using Avalanche.WriteModel.Commands;
using Broadcast;

namespace Avalanche.WriteModel
{
    public class CommandDispatcher : Dispatcher<ICommand>
    {
        public CommandDispatcher(IEventBus eventBus)
        {
            Register<StartTestCommand>(new StartTestCommandHandler(eventBus));
            Register<EndTestCommand>(new EndTestCommandHandler(eventBus));
            Register<TestResultCommand>(new TestResultCommandHandler(eventBus));

            Register<StartupThreadCommand>(new StartupThreadCommandHandler(eventBus));
            Register<EndThreadCommand>(new EndThreadCommandHandler(eventBus));
            Register<IterationCommand>(new IterationCommandHandler(eventBus));

            Register<DeleteTestRunCommand>(new DeleteTestCommandHandler(eventBus));
        }
    }
}
