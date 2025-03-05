using Broadcast;

namespace Avalanche.WriteModel
{
    public interface ICommandHandler : IMessageHandler<ICommand> 
    {
    }
}
