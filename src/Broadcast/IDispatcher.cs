namespace Broadcast
{
    public interface IDispatcher<T> : IMessageBus
    {
        void SendAsync<Tc>(Tc @event) where Tc : class, T;

        void Close();
    }
}
