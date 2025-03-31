namespace Broadcast
{
    public interface IDispatcher<T> : IDisposable
    {
        void SendAsync<Tc>(Tc @event) where Tc : class, T;

        void Close();
    }
}
