using Broadcast;

namespace Avalanche.WriteModel
{
    public interface IEventHandler<in T> : IMessageHandler<T> where T : IEvent
    {
    }
}
