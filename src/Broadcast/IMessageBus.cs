
namespace Broadcast
{
    public interface IMessageBus : IDisposable
    {
        void Register<Tevent>(IMessageHandler<Tevent> handler);

        void Send<Tevent>(Tevent @event);
    }
}
