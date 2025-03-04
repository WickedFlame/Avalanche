using Broadcast;

namespace Avalanche.CommandModel
{
    public interface ICommandHandler : IDispatcherHandler<ICommand> 
    {
    }
}
