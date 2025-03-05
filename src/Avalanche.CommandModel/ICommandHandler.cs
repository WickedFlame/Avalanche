using Broadcast;

namespace Avalanche.CommandModel
{
    public interface ICommandHandler : IMessageHandler<ICommand> 
    {
    }
}
