using Broadcast;

namespace Avalanche.CommandModel
{
    public interface IEventHandler : IMessageHandler<IEvent>
    {
    }
}
