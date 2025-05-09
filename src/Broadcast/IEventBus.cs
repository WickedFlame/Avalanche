
namespace Broadcast
{
    public interface IEventBus : IDisposable
    {
        void Subscribe<Tevent>(IMessageHandler<Tevent> handler);

        void Send<Tevent>(Tevent @event);

        void Publish<Tevent>(string id, DateTime time, Tevent @event) where Tevent : IEvent;
    }
}
