using Broadcast;

namespace Avalanche.CommandModel
{
    public interface IEventHandler : IDispatcherHandler<IEvent>
    {
    }
}
