using Broadcast;

namespace Avalanche.WriteModel
{
    public interface IEventHandler : IMessageHandler<IEvent>
    {
    }
}
